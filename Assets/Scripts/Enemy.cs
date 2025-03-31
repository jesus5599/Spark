using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        initialPosition = transform.position; // Guarda la posición inicial
        
    }

    public void Defeat()
    {
       

        StartCoroutine(Muerto());
       
        
    }
    IEnumerator Muerto()
    {
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
        transform.position = initialPosition; // Restaura la posición inicial
        gameObject.SetActive(true); // Reactiva el enemigo
    }
   
    


}

    


