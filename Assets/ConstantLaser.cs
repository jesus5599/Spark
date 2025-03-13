using UnityEngine;

public class ConstantLaser : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public float maxLaserDistance = 10f; // Distancia máxima del láser
    public Vector3 laserDirection = Vector3.down; // Dirección fija del láser

    public Material normalMaterial; // Material por defecto
    public Material hitMaterial; // Material cuando golpea al jugador

    void Start()
    {
        // Inicializar el LineRenderer y configurarlo
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // Necesitamos dos puntos para el láser (inicio y fin)
    }

    void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
    {
        // Establecer la posición inicial del láser (desde el objeto que lo dispara)
        lineRenderer.SetPosition(0, transform.position);

        // Obtener la dirección fija del láser
        Vector3 direction = transform.TransformDirection(laserDirection);
        Vector3 endPosition = transform.position + direction * maxLaserDistance;

        // Si hay un objeto en el camino, el láser se detiene ahí
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxLaserDistance))
        {
            endPosition = hit.point;

            // Si el láser golpea al jugador
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Controladorjugador>().Muerto(); // Golpea al jugador
                lineRenderer.material = hitMaterial; // Cambia de color al golpear
            }
            else
            {
                // Si no golpea al jugador, vuelve al color normal
                lineRenderer.material = normalMaterial;
            }
        }
        else
        {
            // Si no hay colisión, asegurarse de que el láser tenga el color normal
            lineRenderer.material = normalMaterial;
        }

        // Establecer la posición final del láser (donde termina)
        lineRenderer.SetPosition(1, endPosition);
    }
}
