using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public int checkpointID; // ID del checkpoint al que pertenece este enemigo
    private Vector3 initialPosition; // Guarda la posición inicial del enemigo
    public Color iconColor = Color.red; // Color predeterminado (puedes cambiarlo en el Inspector)
    private Minimap minimap;
    void Start()
    {
        initialPosition = transform.position; // Guarda la posición inicial
        minimap = FindObjectOfType<Minimap>(); // Obtener la referencia al minimapa
    }

    public void Defeat()
    {
        gameObject.SetActive(false); // Desactiva el enemigo cuando es derrotado
        if(minimap != null ) minimap.RemoveEnemyFromMinimap(this);
        
    }

    public void Respawn()
    {
        transform.position = initialPosition; // Restaura la posición inicial
        gameObject.SetActive(true); // Reactiva el enemigo
    }
   
    


}

    


