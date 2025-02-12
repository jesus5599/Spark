using UnityEngine;

public class Puerta : MonoBehaviour
{
    public int puertaID;
    public Vector3 posicionAbierta; // La posición en el espacio local donde se abrirá la puerta
    private Vector3 posicionInicial; // La posición inicial en el espacio local
    private bool estaAbierta = false;

    // Añadir velocidad de movimiento
    public float velocidadDeMovimiento = 2f;
    private float tiempoDeMovimiento = 0f; // Tiempo de transición

    private void Start()
    {
        posicionInicial = transform.localPosition; // Guardamos la posición inicial en el espacio local
        ControladorPuertasYBotones.Instance.RegistrarPuerta(this);
    }

    public void CambiarEstado(bool abrir)
    {
        if (abrir && !estaAbierta)
        {
            StartCoroutine(MoverPuerta(posicionAbierta)); // Mover hacia la posición abierta en espacio local
            estaAbierta = true;
        }
        else if (!abrir && estaAbierta)
        {
            StartCoroutine(MoverPuerta(posicionInicial)); // Mover hacia la posición inicial en espacio local
            estaAbierta = false;
        }
    }

    private System.Collections.IEnumerator MoverPuerta(Vector3 destino)
    {
        while (Vector3.Distance(transform.localPosition, destino) > 0.1f) // Mientras no esté cerca de la posición objetivo en el espacio local
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, destino, Time.deltaTime * velocidadDeMovimiento); // Movimiento suave en espacio local
            yield return null; // Espera hasta el siguiente frame
        }

        transform.localPosition = destino; // Asegura que llegue exactamente a la posición de destino
    }
}
