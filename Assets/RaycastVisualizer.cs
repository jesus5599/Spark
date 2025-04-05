using UnityEngine;

public class RaycastVisualizer : MonoBehaviour
{
    public LineRenderer lineRenderer; // Referencia al LineRenderer
    public float rayDistance = 10f;
    public Vector3 vector3;
    public Vector3 vector3pos;
    public float startwidth,endwidth ;
    void Start()
    {
        // Configuración inicial del LineRenderer
        lineRenderer.positionCount = 2; // Dos puntos: inicio y fin del rayo
        
    }

    void Update()
    {
        lineRenderer.startWidth = startwidth;
        lineRenderer.endWidth = endwidth;
        Vector3 startPoint = vector3pos;
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
