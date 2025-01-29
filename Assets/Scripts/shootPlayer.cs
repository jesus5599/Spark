using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootPlayer : MonoBehaviour
{
    public float speed;
    public GameObject particle;
   
    private void Start()
    {
        
        transform.Rotate(90f,0f,0f);
        StartCoroutine(Destroy());
    }
    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.unscaledDeltaTime);

        if (UnifiedMenuController.isDeath)
        {
            gameObject.SetActive(false);
        }
    }
    IEnumerator Destroy()
    {
        
        yield return new WaitForSeconds(15);
        Destroy(gameObject);
    }
    IEnumerator ShowImpact(Vector3 position, Vector3 normal)
    {
        // Crear el impacto en el punto de colisión con la rotación hacia la normal
        Quaternion rotation = Quaternion.LookRotation(normal);
        GameObject impacto = Instantiate(particle, position, rotation);
        impacto.SetActive(true);

        yield return new WaitForSeconds(0.5f); // Esperar antes de destruir el efecto (ajústalo según sea necesario)

        
        Destroy(gameObject); // Destruir la bala después de mostrar el impacto
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Mostrar siempre el objeto con el que colisiona
        Debug.Log("Choca con: " + collision.gameObject.name);

        // Si el objeto tiene el componente Enemy, derrotarlo
        if (collision.collider.GetComponent<Enemy>() != null)
        {
            collision.collider.GetComponent<Enemy>().Defeat();
            Debug.Log("Es un enemigo. Derrotado.");
        }
        else if (collision.gameObject.CompareTag("Obstaculo"))
        {
            Debug.Log("Choca con un obstáculo.");
            // Lógica para un obstáculo, si es necesario
        }
        else
        {
            Debug.Log("Colisión con otro objeto sin lógica específica.");
        }

        // Obtener el primer punto de contacto
        ContactPoint contact = collision.contacts[0];

        // Obtener la posición del impacto
        Vector3 hitPosition = contact.point;

        // Obtener la normal de la superficie impactada
        Vector3 hitNormal = contact.normal;

        // Desplazar el impacto un poco hacia atrás en la dirección de la normal
        Vector3 adjustedPosition = hitPosition - hitNormal * -0.15f; // Ajusta 0.1f según necesites

        // Iniciar la corrutina con la nueva posición ajustada
        StartCoroutine(ShowImpact(adjustedPosition, hitNormal));
    }


}