using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab específico de este spawner
    public Transform spawnPoint; // Punto de aparición del enemigo
    private GameObject currentEnemy; // Referencia al enemigo actual

    void Start()
    {
        StartCoroutine(CheckAndRespawn());
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Instanciar enemigo y guardar referencia
        currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemy.SetActive(true);
    }

    IEnumerator CheckAndRespawn()
    {
        while (true) // Bucle infinito
        {
            if (currentEnemy == null) // Si el enemigo ha muerto o no existe
            {
                yield return new WaitForSeconds(3f); // Espera 3 segundos antes de respawnear
                SpawnEnemy();
            }
            yield return new WaitForSeconds(1f); // Verificar cada 1 segundo
        }
    }
}
