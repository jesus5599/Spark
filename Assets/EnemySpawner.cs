using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Diferentes tipos de enemigos
    public Transform[] spawnPoints; // Puntos de aparición
    public float initialSpawnRate = 3f; // Tiempo inicial entre spawns
    public float spawnAcceleration = 0.05f; // Aceleración del spawn
    private float currentSpawnRate;
    private int waveCount = 1; // Contador de oleadas

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnRate);

            // Elegir un punto de spawn aleatorio
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomSpawnIndex];

            // Elegir un enemigo basado en la dificultad
            GameObject enemyToSpawn = GetEnemyByDifficulty();

            // Instanciar y activar el enemigo
            GameObject instantiatedEnemy = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
            instantiatedEnemy.SetActive(true); // Asegurarse de que la instancia se active

            // Acelerar el spawn con el tiempo
            currentSpawnRate = Mathf.Max(0.5f, currentSpawnRate - spawnAcceleration);
            waveCount++;
        }
    }

    GameObject GetEnemyByDifficulty()
    {
        // Mayor probabilidad de enemigos fuertes en oleadas altas
        int randomIndex = Random.Range(0, enemyPrefabs.Length);

        if (waveCount > 10 && enemyPrefabs.Length > 2)
            randomIndex = Random.Range(1, enemyPrefabs.Length); // Evita el enemigo más fácil después de la oleada 10

        if (waveCount > 20 && enemyPrefabs.Length > 2)
            randomIndex = enemyPrefabs.Length - 1; // Solo enemigos fuertes después de la oleada 20

        return enemyPrefabs[randomIndex];
    }
}
