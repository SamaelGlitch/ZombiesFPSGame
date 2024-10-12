using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieFollow : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform; // Cambiar a privado, referencia al jugador

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Buscar al jugador en la escena
        Player player = FindObjectOfType<Player>();

        // Verificar si se encontró al jugador y obtener su Transform
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("No se encontró un jugador en la escena.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position); // Mover al jugador
        }
    }
}