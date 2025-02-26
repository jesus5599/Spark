using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public RectTransform minimapPanel;
    public GameObject iconPrefab;
    public float mapSize = 50f; // Ajusta según el tamaño real del mapa

    private Dictionary<Transform, GameObject> enemyIcons = new Dictionary<Transform, GameObject>();

    void Start()
    {
       
    }

    void Update()
    {
        foreach (var enemy in enemyIcons)
        {
            UpdateIcon(enemy.Key);
        }

       
    }

  

    public void RemoveEnemyFromMinimap(Enemy enemy)
    {
        if (enemyIcons.ContainsKey(enemy.transform))
        {
            // Destruir el icono del minimapa
            Destroy(enemyIcons[enemy.transform]);
            enemyIcons.Remove(enemy.transform);
        }
    }

    void UpdateIcon(Transform target)
    {
        if (target == null || !enemyIcons.ContainsKey(target)) return;

        Vector3 offset = target.position - player.position;
        Vector2 minimapPos = new Vector2(offset.x, offset.z) / mapSize;
        minimapPos *= minimapPanel.rect.size / 2f;

        float radius = minimapPanel.rect.size.x / 2f;
        if (minimapPos.magnitude > radius)
        {
            minimapPos = minimapPos.normalized * radius;
        }

        enemyIcons[target].GetComponent<RectTransform>().anchoredPosition = minimapPos;
    }
}
