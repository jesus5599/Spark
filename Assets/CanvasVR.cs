using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasVR : MonoBehaviour
{
    private Transform target;   // Objeto al que sigue la posición
    private Transform targetrot; // Objeto al que sigue la rotación

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindTarget(); // Intenta encontrar el objetivo al activarse
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindTarget(); // Reintentar al cargar una nueva escena
    }

    void FindTarget()
    {
        GameObject targetObject = GameObject.FindWithTag("objetivocanvas");
        GameObject rotateObject = GameObject.FindWithTag("rotacioncanvas");

        if (targetObject != null && rotateObject != null)
        {
            target = targetObject.transform;
            targetrot = rotateObject.transform;
            Debug.Log("Objetivo encontrado: " + target.name);
        }
        else
        {
            Debug.LogWarning("No se encontraron los objetos con las etiquetas. Usando la cámara principal temporalmente.");
            target = Camera.main?.transform;
            targetrot = Camera.main?.transform;
        }
    }

    void Update()
    {
        if (target == null || targetrot == null)
        {
            FindTarget(); // Reintenta en cada frame hasta encontrarlo
        }

        if (target != null && targetrot != null)
        {
            MoveCanvas(target.position, targetrot.rotation);
        }
    }

    public void MoveCanvas(Vector3 newPosition, Quaternion newRotation)
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.transform.position = newPosition;
            canvas.transform.LookAt(targetrot);
            canvas.transform.Rotate(0, 180, 0); // Corrige la orientación para que no se vea al revés

            //Debug.Log("Canvas movido a: " + newPosition + " y rotado hacia el objetivo.");
        }
    }
}
