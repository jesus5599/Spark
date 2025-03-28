using UnityEngine;

public class RaycastVisualizer : MonoBehaviour
{
    public LineRenderer lineRenderer; // Referencia al LineRenderer
    public float rayDistance = 10f;
    public Vector3 vector3;
    void Start()
    {
        // Configuración inicial del LineRenderer
        lineRenderer.positionCount = 2; // Dos puntos: inicio y fin del rayo
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        Vector3 startPoint = new Vector3(0, .9f, 0);
        Vector3 endPoint = startPoint + vector3 * rayDistance;

        // Dibujar la línea
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        // Comprobar colisión con Raycast
        if (Physics.Raycast(startPoint, vector3, out RaycastHit hit, rayDistance))
        {
            Debug.Log("Impacto con: " + hit.collider.name);
            lineRenderer.SetPosition(1, hit.point); // Ajusta la línea al punto de impacto
        }
    }
}
