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

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("ObjetivoBala").transform;

        // Hacer que el láser comience apuntando hacia abajo
        predictedTarget = transform.position + Vector3.down * maxLaserDistance;

        // Asignar material inicial (tracking)
        lineRenderer.material = trackingMaterial;
        AdjustShootVelocity();
        LoadDifficulty();
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
    private void AdjustShootVelocity()
    {
        // Ajusta los tiempos de disparo según la dificultad
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
               delayFactor = 3;
                break;
            case Difficulty.Normal:
                delayFactor = 4;
                break;
            case Difficulty.Hard:
                delayFactor = 5;
                break;
        }
    }
    private void LoadDifficulty()
    {
        // Cargar la dificultad desde PlayerPrefs. Si no se ha guardado, se asume dificultad Normal.
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            int difficultyValue = PlayerPrefs.GetInt("Difficulty");
            currentDifficulty = (Difficulty)difficultyValue;
        }
        else
        {
            currentDifficulty = Difficulty.Normal; // Valor por defecto
        }
    }
    private void OnEnable()
    {
        AdjustShootVelocity();
        LoadDifficulty();
    }
}
