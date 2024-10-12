using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject enemyPrefab; // Prefab del enemigo que se va a spawnear
    [SerializeField] private Transform spawnPoint; // Punto desde el que se spawnean los enemigos
    [SerializeField] private float spawnInterval = 2f; // Intervalo de tiempo entre cada spawn

    private bool isSpawning = true;

    private void Start()
    {
        // Iniciar la corrutina para spawnear enemigos de manera indefinida
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (isSpawning)
        {
            // Instanciar un enemigo en el punto de spawn con la rotación predeterminada
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Esperar el intervalo antes de spawnear otro enemigo
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
