using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class Enemy : MonoBehaviour
{
    public int checkpointID; // ID del checkpoint al que pertenece este enemigo
    private Vector3 initialPosition; // Guarda la posición inicial del enemigo
    private Animator animate;
    public EnemyAI enemy;
    public APUNTADO apuntar;
    public CapsuleCollider capsuleCollider;
    public CapsuleCollider capsuleTrigger;
    public Rigidbody rb;
   
    
    public AudioSource audioSource;
    public AudioClip[] sonidos; // Aquí arrastras tus sonidos desde el editor
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        initialPosition = transform.position; // Guarda la posición inicial
    }
    void Start()
    { 
        CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();

        foreach (CapsuleCollider col in colliders)
        {
            if (col.isTrigger)
            {
                capsuleTrigger = col; // Asigna el Trigger
            }
            else
            {
                capsuleCollider = col; // Asigna el Collider normal
            }
        }
        rb = GetComponentInChildren<Rigidbody>();
        apuntar = GetComponentInChildren<APUNTADO>();
        enemy = GetComponent<EnemyAI>();
        animate = GetComponent<Animator>();
       
        
    }

    public void Defeat()
    {
       

        StartCoroutine(Muerto());
       
        
    }
    IEnumerator Muerto()
    {
        ReproducirSonidoAleatorio();
        apuntar.muerto();
        enemy.Muerto();
        capsuleCollider.enabled = false;
        capsuleTrigger.enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(3f);
        rb.useGravity = true;
        capsuleCollider.enabled = true;
        capsuleTrigger.enabled = true;
        gameObject.SetActive(false); // Desactiva el enemigo cuando es derrotado    
    }
    public void Respawn()
    {
        rb.useGravity = true;
        transform.position = initialPosition; // Restaura la posición inicial
        gameObject.SetActive(true); // Reactiva el enemigo
    }


    public void ReproducirSonidoAleatorio()
    {
        if (audioSource != null && sonidos != null && sonidos.Length > 0)
        {
            int index = Random.Range(0, sonidos.Length);
            audioSource.PlayOneShot(sonidos[index]);
        }
    }

}

    


