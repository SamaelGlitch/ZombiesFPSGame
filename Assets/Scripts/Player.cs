using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")] // Atributos visibles de Movimiento
    [SerializeField] private float speed; // Velocidad a la que se mueve el jugador.
    [SerializeField] private float jumpHeight; // Altura del salto en unidades de Unity.
    [SerializeField] private float rotationSensitivity; // Sensibilidad de rotación con el mouse.

    [Header("Shooting")] // Atributos visibles de Disparo
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala
    [SerializeField] private Transform shootPoint; // Punto de origen del disparo
    [SerializeField] private float shootForce = 20f; // Fuerza del disparo
    [SerializeField] private float fireRate = 0.5f; // Cadencia de disparo (0.5 segundos entre disparos)
    private float nextFireTime = 0f; // Tiempo que debe esperar antes de poder disparar de nuevo

    [Header("Player Stats")]
    [SerializeField] private int maxHealth = 10; // Vida máxima del jugador
    private int currentHealth; // Vida actual del jugador

    private Transform head; // Objeto hijo que representa la cabeza que permite rotar la cabeza en X.
    private readonly float gravity = -9.8f; // Gravedad (es una constante).
    private CharacterController character; // Character Controller en el Game Object que usa este Script.
    private Control input; // Objeto de Control (Input Actions).
    private float velocityY = 0; // velocidad en Y actual del jugador.

    private bool isInvulnerable = false; // Indica si el jugador es invulnerable (después de recibir daño)
    private float invulnerabilityDuration = 1f; // Duración de la invulnerabilidad en segundos

    // Contador de muertes
    private int killCount = 0;


    // Awake se ejecuta antes de cualquier cosa.
    private void Awake()
    {
        // Inicializar la variable de input.
        input = new Control();

        // Acción que se realiza en el salto.
        input.FPS.Jump.performed += ctx =>
        {
            if (character.isGrounded)
            {
                velocityY += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            }
        };

        Cursor.lockState = CursorLockMode.Locked; // Desaparecer el mouse de la pantalla (podemos sacarlo con Esc)
    }

    // OnEnable se ejecuta al activar el objeto.
    private void OnEnable()
    {
        input.FPS.Enable();
    }

    //OnDisable se ejecuta al desactivar el objeto.
    private void OnDisable()
    {
        input.FPS.Disable();
    }

    private void Shoot()
    {
        // Comprobar si el jugador puede disparar (según el fireRate)
        if (Time.time >= nextFireTime)
        {
            // Instanciar la bala en el punto de disparo con la misma rotación que el jugador.
            GameObject Bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            // Obtener el Rigidbody de la bala y asegurarnos que tenga uno.
            Rigidbody rb = Bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar la fuerza en la dirección del punto de disparo hacia adelante.
                rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
            }

            Destroy(Bullet, 3f);

            // Actualizar el tiempo para el próximo disparo.
            nextFireTime = Time.time + fireRate;
        }
    }


    // Start se ejecuta antes del primer frame.
    private void Start()
    {
        character = GetComponent<CharacterController>();
        head = transform.GetChild(0); // Busca el primer Transform dentro de este Transform (o sea el primer hijo).
        // Inicializar la salud del jugador.
        currentHealth = maxHealth;
    }

    // Update se ejecuta cada frame.
    private void Update()
    {
        // Detectar si se ha presionado el botón de disparo.
        if (input.FPS.Shoot.triggered)
        {
            Shoot();
        }

    }

    // FixedUpdate se ejecuta cada fixedFrame que es un framing que no cambia aunque la computadora tenga más o menos FPS.
    private void FixedUpdate()
    {
        Vector2 mov = input.FPS.Move.ReadValue<Vector2>(); // Recuperamos el valor de Move.

        Vector2 rot = input.FPS.Look.ReadValue<Vector2>(); // Recuperamos el valor de Look.

        // Si nuestro personaje está en el suelo y la velocidad es menor a 0, se detiene.
        if (character.isGrounded && velocityY < 0)
            velocityY = 0;

        // Rotar el cuerpo en X y la cabeza en Y.
        transform.Rotate(Vector3.up * rot.x * rotationSensitivity);
        head.Rotate(Vector3.right * rot.y * rotationSensitivity);

        // Configurar vector de movimiento.
        Vector3 velocityXZ = transform.rotation * new Vector3(mov.x, 0, mov.y);

        // Mover el personaje en X y Z.
        character.Move(velocityXZ * speed);

        // Configurar la velocidad en Y
        velocityY += gravity * Time.deltaTime;

        // Mover el personaje en Y.
        character.Move(velocityY * Vector3.up * Time.deltaTime);
    }

    // Función que maneja el daño recibido por el jugador.
    private void TakeDamage(int damage)
    {
        if (!isInvulnerable)
        {
            currentHealth -= damage;
            Debug.Log("Player hit! Health: " + currentHealth);

            // Si la salud llega a 0 o menos, el jugador muere.
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // Activar la invulnerabilidad temporalmente.
                StartCoroutine(InvulnerabilityCoroutine());
            }
        }
    }

    // Corrutina que controla el tiempo de invulnerabilidad
    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    // Función que destruye al jugador cuando muere.
    private void Die()
    {
        Debug.Log("Player is dead!");
        Debug.Log("Total kills: " + killCount);
        // Puedes añadir lógica adicional aquí para manejar la muerte del jugador (reiniciar el nivel, mostrar un menú, etc.)
        Destroy(gameObject);
    }

    // Detectar colisión con el enemigo.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) // Asegúrate de que los enemigos tengan el tag "Enemy"
        {
            TakeDamage(1); // El jugador recibe 1 punto de daño.
        }
    }

    // Función para aumentar el contador de muertes
    public void AddKill()
    {
        killCount++;
        Debug.Log("Kills: " + killCount);
    }
}
