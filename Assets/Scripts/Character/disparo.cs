using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disparo : MonoBehaviour
{
    
    public GameObject targetObject;  // Target object
    public GameObject brazo;
    public Rigidbody misil;  // Rigidbody for the missile (change to Rigidbody)
    public Transform lanzador;  // The launcher's position and rotation 
    public float veldisparo, tiempoDeRecarga;  // Speed at which the missile is shot
    public static bool disparoarma;
    public static Vector3 puntoimpacto;
    public int municioninicial, municionactual;
    public bool recarga;
    public GameObject bala1, bala2, bala3, bala4, bala5, bala6;
    public AudioSource audioSource; // Componente AudioSource para reproducir sonido
    public AudioClip disparoClip,recargaClip;   // Sonido del disparo
    public GameObject flash;
    // Start is called before the first frame update
    void Start()
    {
        disparoarma = false;
       
        municionactual = municioninicial;
        recarga = true;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
       

        if (municionactual <= 0 && recarga==true)
        {
            StartCoroutine(Reload()); 
            recarga = false;
        }

        if (disparoarma == true && municionactual > 0)
        {
            StartCoroutine(ShowFlash()); 
            Shoot();
        }
    }
    void LateUpdate()
    {
        // Calculate the direction of the missile in the world space
        Vector3 fwd = lanzador.TransformDirection(Vector3.forward);


        // Get the direction vector from the launcher to the target 
        Vector3 difference = targetObject.transform.position - transform.position;
        // Calculate the rotation needed to face the target in 3D space
        Quaternion rotation = Quaternion.LookRotation(difference);
        brazo.transform.rotation = rotation;

     
    }
    private void Shoot()
    {
        if (audioSource != null && disparoClip != null)
        {
            audioSource.PlayOneShot(disparoClip);
        }
        disparoarma = false;
        municionactual = municionactual -1;
        BalasVisibles();
        
       
        Rigidbody misilInstanc;
        misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
        misilInstanc.transform.LookAt(puntoimpacto);
        misilInstanc.gameObject.SetActive(true);
        
       



    }
    private void BalasVisibles()
    {
        if (municionactual == 6)
        {
            bala1.gameObject.SetActive(true);
            bala2.gameObject.SetActive(true);
            bala3.gameObject.SetActive(true);
            bala4.gameObject.SetActive(true);
            bala5.gameObject.SetActive(true);
            bala6.gameObject.SetActive(true);
        }
        if (municionactual == 5 )
        {
             bala1.gameObject.SetActive(false);
        }
        if (municionactual == 4)
        {
            bala2.gameObject.SetActive(false);
        }
        if (municionactual == 3)
        {
            bala3.gameObject.SetActive(false);
        }
        if (municionactual == 2)
        {
            bala4.gameObject.SetActive(false);
        }
        if (municionactual == 1)
        {
            bala5.gameObject.SetActive(false);
        }
        if (municionactual <= 0)
        {
            bala6.gameObject.SetActive(false);
        }



    }
    IEnumerator Reload()
    { // Reproducir el sonido del disparo
        if (audioSource != null && recargaClip != null)
        {
            audioSource.PlayOneShot(recargaClip);
        }
        yield return new WaitForSeconds(tiempoDeRecarga);
        municionactual = municioninicial;
        BalasVisibles();
        recarga =true;
        disparoarma = false;


    }
    IEnumerator ShowFlash()
    {
        GameObject destello;
        destello = Instantiate(flash, lanzador.position, lanzador.rotation);
        destello.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);

    }
    public void mort()
    {
        municionactual = municioninicial;
        BalasVisibles();
        recarga = true;
        disparoarma = false;

    }
}

