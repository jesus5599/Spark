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
    IEnumerator ShowImpact()
    {
        GameObject impacto;
        impacto = Instantiate(particle, transform.position, transform.rotation);
        impacto.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        Destroy(gameObject);
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

        // Llamar siempre a ShowImpact()
        StartCoroutine(ShowImpact());
    }


}