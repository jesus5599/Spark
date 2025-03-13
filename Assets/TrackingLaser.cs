using UnityEngine;
using System.Collections;

public class TrackingLaser : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform player;
    private Vector3 predictedTarget;

    public float trackingTime = 2f; // Tiempo que el láser sigue antes de disparar
    public float laserDuration = 0.5f; // Tiempo que el láser está activo
    public float maxLaserDistance = 10f; // Distancia máxima del láser
    public float delayFactor = 0.5f; // Qué tan lento sigue al jugador

    public Material trackingMaterial; // Material cuando rastrea
    public Material firingMaterial; // Material cuando dispara
    public LayerMask Layer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("ObjetivoBala").transform;

        // Hacer que el láser comience apuntando hacia abajo
        predictedTarget = transform.position + Vector3.down * maxLaserDistance;

        // Asignar material inicial (tracking)
        lineRenderer.material = trackingMaterial;

        StartCoroutine(TrackAndFire());
    }

    IEnumerator TrackAndFire()
    {
        float elapsedTime = 0f;

        while (elapsedTime < trackingTime)
        {
            // Sigue la posición del jugador con retraso
            predictedTarget = Vector3.Lerp(predictedTarget, player.position, delayFactor * Time.deltaTime);
            UpdateLaser(predictedTarget, true);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Cambiar a material de disparo antes de activar el láser
        lineRenderer.material = firingMaterial;
        UpdateLaser(predictedTarget, false);
        yield return new WaitForSeconds(laserDuration);

        // Desactivar el láser
        lineRenderer.enabled = false;
        Destroy(gameObject, 0.5f); // Eliminar después de un pequeño delay
    }

    void UpdateLaser(Vector3 targetPosition, bool charging)
    {
        // Posición inicial del láser (desde el jefe)
        lineRenderer.SetPosition(0, transform.position);

        // Posición final (hacia la última posición rastreada del jugador)
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 endPosition = transform.position + direction * maxLaserDistance;

        // Si hay un objeto en el camino, el láser se detiene ahí
        int excludeParryLayer = ~Layer.value;
        
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxLaserDistance,excludeParryLayer))
        {
            endPosition = hit.point;
            if (!charging && hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Controladorjugador>().Muerto(); // Golpea al jugador
            }
        }

        lineRenderer.SetPosition(1, endPosition);
    }
}
