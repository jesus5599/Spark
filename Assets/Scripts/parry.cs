using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class parry : MonoBehaviour
{

    public GameObject targetObject;  // Target object

    public Rigidbody misil;  // Rigidbody for the missile (change to Rigidbody)
    public Transform lanzador;  // The launcher's position and rotation 

    public  Vector3 puntoimpacto;
    public LayerMask ParryLayer;
    public AudioClip parryClip;
    public AudioSource audioSource;
    public bool vr;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    public void Shoot()
    {
        if (audioSource != null && parryClip != null)
        {
            audioSource.PlayOneShot(parryClip);
        }


        int excludeParryLayer = ~ParryLayer.value;
        RaycastHit hit;
        Ray rayo;

        // Verificar si es VR o no y definir el rayo correspondiente
        if (vr)
        {
            Transform puntoDisparo = lanzador.transform; // Asegurar que puntapistola est? asignado
            rayo = new Ray(puntoDisparo.position, puntoDisparo.forward);
        }
        else
        {
            Vector3 puntopantalla = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
            rayo = Camera.main.ScreenPointToRay(puntopantalla);
        }

        Physics.Raycast(rayo, out hit, 1000, ~ParryLayer.value);
        puntoimpacto = hit.point;
        Rigidbody misilInstanc;
        misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
        misilInstanc.transform.LookAt(puntoimpacto);
        misilInstanc.gameObject.SetActive(true);

    }

}
