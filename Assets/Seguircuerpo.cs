using UnityEngine;

public class SeguirCuerpo : MonoBehaviour
{
    public GameObject Character; // El objeto cuya posición quieres copiar
    public Transform camara; // La cámara a la que el objeto A debe seguir
    public GameObject Character2; // El objeto cuya posición quieres copiar
    public GameObject xrrig; // El objeto cuya posición quieres copiar
    void Update()
    {
      
        // Hacer que el Character rote horizontalmente siguiendo la rotación de la cámara
        if (camara != null && Character != null)
        {
            // Calculamos la dirección hacia la cámara, pero solo en el plano horizontal
            Vector3 direccion = (camara.position+Vector3.forward) - Character.transform.position;
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
    void LateUpdate()
    {
       // Copiar la posición X y Z de la cámara, pero mantener la Y del Character
        if (Character != null)
        {
            Vector3 newPosition = new Vector3(
                Camera.main.transform.position.x,
                Character.transform.position.y,
                Camera.main.transform.position.z
            );

            Character.transform.position = newPosition;
        }
        // Copiar la posición X y Z de la cámara, pero mantener la Y del Character
        if (Character2 != null)
        {
            Vector3 newPosition = new Vector3(
                xrrig.transform.position.x,
                Character2.transform.position.y,
                xrrig.transform.position.z
            );

            Character2.transform.position = newPosition;
        }
    }

}
