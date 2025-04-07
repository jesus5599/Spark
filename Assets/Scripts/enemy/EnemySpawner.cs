using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Diferentes tipos de enemigos (ordenados de débil a fuerte)
    public Transform[] spawnPoints; // Puntos de aparición
    public float initialSpawnRate = 3f; // Tiempo inicial entre spawns
    public float spawnAcceleration = 0.05f; // Aceleración del spawn
    private float currentSpawnRate;
    public int waveCount = 1; // Contador de oleadas
   
    public Minimap minimap; // Referencia al minimapa


    void Start()
    {
    minimap = FindObjectOfType<Minimap>(); // Buscar el minimapa en la escena
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

            // Elegir un enemigo basado en la dificultad progresiva
            GameObject enemyToSpawn = GetEnemyByDifficulty();

            // Instanciar y activar el enemigo
            GameObject instantiatedEnemy = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
            instantiatedEnemy.SetActive(true); // Asegurar que la instancia se active

            // Acelerar el spawn con el tiempo
            currentSpawnRate = Mathf.Max(0.5f, currentSpawnRate - spawnAcceleration);
            waveCount++;           
           
        }
    }

    GameObject GetEnemyByDifficulty()
    {
        float[] probabilities = GetEnemyProbabilities();

        float randomValue = Random.Range(0f, 100f);
        float cumulative = 0f;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            cumulative += probabilities[i];
            if (randomValue <= cumulative)
            {
                return enemyPrefabs[i];
            }
        }

        return enemyPrefabs[0]; // Fallback
    }

    float[] GetEnemyProbabilities()
    {
        // Definir probabilidades base (100% en total)
        float[] probabilities = new float[enemyPrefabs.Length];

        if (enemyPrefabs.Length == 3)
        {
            // Calculamos la progresión de probabilidades según la oleada
            float weakProb = Mathf.Max(10f, 70f - waveCount * 2f);   // Disminuye con el tiempo
            float mediumProb = Mathf.Clamp(20f + waveCount * 1f, 20f, 50f); // Aumenta lentamente
            float strongProb = 100f - (weakProb + mediumProb); // Lo que falta para llegar a 100%

            probabilities[0] = weakProb;
            probabilities[1] = mediumProb;
            probabilities[2] = strongProb;
        }
        else
        {
            // Si hay más o menos enemigos, distribuir de forma personalizada
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                probabilities[i] = 100f / enemyPrefabs.Length;
            }
        }

        return probabilities;
    }
}
