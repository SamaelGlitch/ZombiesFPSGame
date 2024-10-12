using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Attributes")]
    [SerializeField] private int maxHealth = 3; // Vida máxima del enemigo
    private int currentHealth; // Vida actual del enemigo

    private Player player; // Referencia al script del jugador

    // Awake se ejecuta antes de que empiece el juego.
    private void Awake()
    {
        // Inicializar la vida actual del enemigo al máximo.
        currentHealth = maxHealth;

        // Encontrar al jugador en la escena
        player = FindObjectOfType<Player>(); // Asegúrate de que haya solo un jugador en la escena
    }

    // Método que se llama cuando otro objeto colisiona con este GameObject
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que ha colisionado es una bala
        if (other.gameObject.CompareTag("Bullet")) // Asegúrate de que tus balas tengan el tag "Bullet"
        {
            // Recibir daño (1 punto por impacto de bala)
            TakeDamage(1);

            // Destruir la bala al impactar
            Destroy(other.gameObject);
        }
    }

    // Función para reducir la salud del enemigo
    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Zombie hit! Health: " + currentHealth);

        // Si la vida llega a 0, destruir al enemigo
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Función que destruye al enemigo
    private void Die()
    {
        Debug.Log("Zombie destroyed!");
        
        // Llamar al método AddKill() del jugador
        if (player != null)
        {
            player.AddKill(); // Incrementar el contador de muertes del jugador
        }

        // Destruir al enemigo
        Destroy(gameObject);
    }
}