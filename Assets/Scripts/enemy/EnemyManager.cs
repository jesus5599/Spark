using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Enemy[] enemies; // Lista de todos los enemigos en la escena

    void Start()
    {
        enemies = FindObjectsOfType<Enemy>(); // Encuentra todos los enemigos en la escena
    }

    public void RespawnEnemies(int currentCheckpointID)
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.checkpointID >= currentCheckpointID) // Solo reaparece enemigos del checkpoint actual o posteriores
            {
                enemy.Respawn();
            }
        }
    }
}

