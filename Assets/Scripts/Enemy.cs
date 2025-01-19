using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public int checkpointID; // ID del checkpoint al que pertenece este enemigo
    private Vector3 initialPosition; // Guarda la posición inicial del enemigo
   
    void Start()
    {
        initialPosition = transform.position; // Guarda la posición inicial
    }

    public void Defeat()
    {
        gameObject.SetActive(false); // Desactiva el enemigo cuando es derrotado
    }

    public void Respawn()
    {
        transform.position = initialPosition; // Restaura la posición inicial
        gameObject.SetActive(true); // Reactiva el enemigo
    }
    
}

    


