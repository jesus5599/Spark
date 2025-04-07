using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HomingMissile : MonoBehaviour
{
    private Transform target;
    public float speed = 5f;
    public float rotationSpeed = 200f;
    public float pursuitDuration = 5f; // Tiempo en segundos antes de dejar de perseguir
    private float pursuitTimer = 0f;   // Contador de tiempo
    public GameObject misil;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    public GameObject particle;
    public GameObject flash;
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        pursuitTimer = 0f; // Reiniciar el temporizador cuando se asigna un nuevo objetivo
    }

    void Update()
    {
        if (misil == null) Destroy(gameObject);
        pursuitTimer += Time.deltaTime; // Aumentar el contador

        if (target != null && pursuitTimer < pursuitDuration)
        {
            // Dirección hacia el objetivo
            Vector3 direction = target.position - transform.position;
            direction.Normalize();

            // Calcular la rotación deseada
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Aplicar una rotación suave hacia el objetivo
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Mover el misil hacia adelante en la dirección en la que apunta
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            // Cuando el tiempo se acaba, solo avanza en línea recta sin girar
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (UnifiedMenuController.isDeath)
        {
            gameObject.SetActive(false);
        }
    }  
    private void Start()
    {
        
        StartCoroutine(ShowFlash());
        AdjustShootVelocity();
        LoadDifficulty();

    }
   
    IEnumerator Destroy()
    {

        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }

    IEnumerator ShowFlash()
    {
        GameObject destello;
        destello = Instantiate(flash, transform.position, transform.rotation);
        destello.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);

    }
    IEnumerator ShowImpact(Vector3 position, Vector3 normal)
    {
        // Crear el impacto en el punto de colisión con la rotación hacia la normal
        Quaternion rotation = Quaternion.LookRotation(normal);
        GameObject impacto = Instantiate(particle, position, rotation);
        impacto.SetActive(true);

        yield return new WaitForSeconds(0.5f); // Esperar antes de destruir el efecto (ajústalo según sea necesario)


        Destroy(gameObject); // Destruir la bala después de mostrar el impacto
    }

    private void OnCollisionEnter(Collision collision)
    {
       
            // Obtener el primer punto de contacto
            ContactPoint contact = collision.contacts[0];

            // Obtener la posición del impacto
            Vector3 hitPosition = contact.point;

            // Obtener la normal de la superficie impactada
            Vector3 hitNormal = contact.normal;

            // Desplazar el impacto un poco hacia atrás en la dirección de la normal
            Vector3 adjustedPosition = hitPosition - hitNormal * -0.15f; // Ajusta 0.1f según necesites

            // Iniciar la corrutina con la nueva posición ajustada
            StartCoroutine(ShowImpact(adjustedPosition, hitNormal));
            misil.gameObject.SetActive(false);
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Parry"))
        {
            // Obtener la posición del impacto
            Vector3 hitPosition = other.ClosestPoint(transform.position);

            // Calcular la normal de la superficie simulada
            Vector3 hitNormal = (transform.position - hitPosition).normalized;

            // Desplazar el impacto un poco hacia atrás en la dirección de la normal
            Vector3 adjustedPosition = hitPosition - hitNormal * -0.15f; // Ajusta 0.1f según necesites

            // Iniciar la corrutina con la nueva posición y normal ajustada
            StartCoroutine(ShowImpact(adjustedPosition, hitNormal));
        }


        other.GetComponent<Collider>().GetComponent<parry>()?.Shoot();


    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Plataforma") && !other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
    private void AdjustShootVelocity()
    {
        // Ajusta los tiempos de disparo según la dificultad
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                speed = 7.5f;
                break;
            case Difficulty.Normal:
                speed = 10;
                break;
            case Difficulty.Hard:
                speed = 15;
                break;
        }
    }
    private void LoadDifficulty()
    {
        // Cargar la dificultad desde PlayerPrefs. Si no se ha guardado, se asume dificultad Normal.
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            int difficultyValue = PlayerPrefs.GetInt("Difficulty");
            currentDifficulty = (Difficulty)difficultyValue;
        }
        else
        {
            currentDifficulty = Difficulty.Normal; // Valor por defecto
        }
    }
    private void OnEnable()
    {
        AdjustShootVelocity();
        LoadDifficulty();
    }
}
