using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APUNTADO : MonoBehaviour
{
    float timeAux;
    public GameObject targetObject;  // Objeto que rotará en Y
    public GameObject enemy;  // Objeto que rotará en Y
    public Rigidbody misil;  // Rigidbody para el misil
    public Transform lanzador;  // Posición y rotación del lanzador
    public Transform seguir;  // Posición del objetivo a seguir
    public float veldisparo;  // Velocidad de disparo del misil
    public float rangeX = 6f;  // Rango horizontal en el eje X
    public float rangeY = 6f;  // Rango vertical en el eje Y
    public float rangeZ = 6f;  // Rango de profundidad en el eje Z

    // Start is called before the first frame update
    void Start()
    {
        timeAux = Time.time;
    }

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
            transform.rotation = rotation;
            // Aplicar la rotación en Y al targetObject
            enemy.transform.rotation = Quaternion.Euler(0, rotationY.eulerAngles.y, 0);


            // Si ha pasado suficiente tiempo, disparar el misil
            if (Time.time - timeAux > 1.5f)
            {
                Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
                misilInstanc.gameObject.SetActive(true);
                
                timeAux = Time.time;  // Reiniciar el tiempo
            }
        }
    }
}
