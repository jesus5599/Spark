using System.Collections;
using System.Collections.Generic;
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
        Vector3 puntopantalla = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Ray rayo = Camera.main.ScreenPointToRay(puntopantalla);
        RaycastHit hit;
        Physics.Raycast(rayo, out hit, 1000, ~ParryLayer.value);
        puntoimpacto = hit.point;
        Rigidbody misilInstanc;
        misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
        misilInstanc.transform.LookAt(puntoimpacto);
        misilInstanc.gameObject.SetActive(true);





    }

}
