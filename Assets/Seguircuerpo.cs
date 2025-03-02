using UnityEngine;

public class SeguirCuerpo : MonoBehaviour
{
    public GameObject Character; // El objeto cuya posición quieres copiar
    public Transform camara; // La cámara a la que el objeto A debe seguir

    void Update()
    {
        // Copiar la posición del Character
        if (Character != null)
        {
            transform.position = Character.transform.position;
        }

        // Hacer que el Character rote horizontalmente siguiendo la rotación de la cámara
        if (camara != null && Character != null)
        {
            // Calculamos la dirección hacia la cámara, pero solo en el plano horizontal
            Vector3 direccion = camara.position - Character.transform.position;
            direccion.y = 0; // Ignorar la rotación en el eje Y para mantener la rotación solo en el plano horizontal

            // Verificar si la dirección es válida (evitar división por cero)
            if (direccion != Vector3.zero)
            {
                // Calculamos la rotación hacia la cámara en el plano horizontal (eje Y)
                Quaternion rotacion = Quaternion.LookRotation(direccion);
                Character.transform.rotation = Quaternion.Euler(0, rotacion.eulerAngles.y, 0); // Solo rotación en Y
            }
        }
    }
}
