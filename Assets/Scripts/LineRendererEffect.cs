using System;
using System.Collections;
using UnityEngine;

public class LineRendererProgress : MonoBehaviour
{
    public LineRenderer[] lineRenderers; // Array para almacenar los LineRenderers
    public static float delay = 0.1f; // Tiempo entre cada paso de carga/descarga

    private Vector3[][] originalPositions; // Posiciones originales de los LineRenderers

    void Start()
    {
        // Guardar las posiciones originales para cada LineRenderer
        originalPositions = new Vector3[lineRenderers.Length][];
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            LineRenderer line = lineRenderers[i];
            originalPositions[i] = new Vector3[line.positionCount];
            line.GetPositions(originalPositions[i]);
        }
    }

    // Función para cargar los LineRenderers (del primero al último punto)
    public void StartLoading()
    {
        StartCoroutine(LoadBars());
    }

    // Función para descargar los LineRenderers (del último al primer punto)
    public void StartUnloading()
    {
        StartCoroutine(UnloadBars());
    }
    public void PointsToOrigin()
    {
        StartCoroutine(origin());
    }
    private IEnumerator origin()
    {
        for (int step = 0; step < originalPositions[0].Length; step++)
        {
            foreach (LineRenderer line in lineRenderers)
            {
                // Restaurar la posición original punto por punto
                line.SetPosition(step, originalPositions[Array.IndexOf(lineRenderers, line)][step]);
            }

        }
        yield return new WaitForSeconds(0);
    }
    // Corrutina para cargar los LineRenderers
    private IEnumerator LoadBars()
    {
        for (int step = 0; step < originalPositions[0].Length; step++)
        {
            foreach (LineRenderer line in lineRenderers)
            {
                // Restaurar la posición original punto por punto
                line.SetPosition(step, originalPositions[Array.IndexOf(lineRenderers, line)][step]);
            }
            yield return new WaitForSeconds(delay);
        }
    }

    // Corrutina para descargar los LineRenderers
    private IEnumerator UnloadBars()
    {
        for (int step = originalPositions[0].Length - 1; step >= 0; step--)
        {
            foreach (LineRenderer line in lineRenderers)
            {
                // Mover el punto actual a la posición original del primer punto
                line.SetPosition(step, originalPositions[Array.IndexOf(lineRenderers, line)][0]);
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
