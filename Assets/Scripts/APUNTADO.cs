using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APUNTADO : MonoBehaviour
{
    
    public GameObject gun;  // Objeto que rotará en XZ
    public GameObject enemy;  // Objeto que rotará en Y
    public Rigidbody misil;  // Rigidbody para el misil
    public Transform lanzador;  // Posición y rotación del lanzador
    public Transform seguir;  // Posición del objetivo a seguir
    public float rangeX = 6f;  // Rango horizontal en el eje X
    public float rangeY = 6f;  // Rango vertical en el eje Y
    public float rangeZ = 6f;  // Rango de profundidad en el eje Z
    public bool pistol, submachinegun, argun;
    public float pistoldelay, submachinegundelay, argundelay;
    public AudioSource audioSource; // Componente AudioSource para reproducir sonido
    public AudioClip ArClip, SubmachineClip, GunClip;   // Sonido de disparo
    // Update is called once per frame
    void Update()
    {        
        // Verificar si el lanzador está dentro del rango en los ejes X, Y y Z
        if (Mathf.Abs(transform.position.x - seguir.position.x) <= rangeX &&
            Mathf.Abs(transform.position.y - seguir.position.y) <= rangeY &&
            Mathf.Abs(transform.position.z - seguir.position.z) <= rangeZ)
        {
            // Calcular la dirección desde el lanzador hacia el objetivo
            Vector3 difference = seguir.position - transform.position;
            
            // Calcular solo la rotación necesaria en Y
            Vector3 lookDirection = new Vector3(difference.x, 0, difference.z);
            
            Quaternion rotationY = Quaternion.LookRotation(lookDirection);
            Quaternion rotation = Quaternion.LookRotation(difference);
            gun.transform.rotation = rotation;
            // Aplicar la rotación en Y al targetObject
            enemy.transform.rotation = Quaternion.Euler(0, rotationY.eulerAngles.y, 0);
            
                if (pistol == true)
                {
                StartCoroutine(Pistolshot());
                }                
                else if (submachinegun == true)
                {
                StartCoroutine(Submachineshot());                
                }
                else if (argun == true)
                {
                StartCoroutine(ARshot());                
                }            
        }
    }
    private IEnumerator Pistolshot()
    {
        pistol = false;
        if (audioSource != null && GunClip != null)
        {
            audioSource.PlayOneShot(GunClip);
        }
        Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
            misilInstanc.gameObject.SetActive(true);
            yield return new WaitForSeconds(pistoldelay);        
            pistol = true;

    }
    private IEnumerator ARshot()
    {
        if (audioSource != null && ArClip != null)
        {
            audioSource.PlayOneShot(ArClip);
        }
        argun = false;
        for (int i = 0; i < 3; i++)
        {
            Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
            misilInstanc.gameObject.SetActive(true);
           yield return new WaitForSeconds(argundelay);

        }
        yield return new WaitForSeconds(argundelay*10);
        argun = true;        
    }
    private IEnumerator Submachineshot()
    {
        if (audioSource != null && SubmachineClip != null)
        {
            audioSource.PlayOneShot(SubmachineClip);
        }
        submachinegun = false;
        for (int i = 0; i < 10; i++)
        {
            Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
            misilInstanc.gameObject.SetActive(true);

        yield return new WaitForSeconds(submachinegundelay);
        }
        yield return new WaitForSeconds(submachinegundelay*50);
        submachinegun = true;        
    }
}

