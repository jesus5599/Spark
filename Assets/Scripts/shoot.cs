using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    public float speed;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    public GameObject particle;
    public GameObject flash;
    private void Start()
    {
        StartCoroutine(Destroy());
        StartCoroutine(ShowFlash());
        AdjustShootVelocity();
        LoadDifficulty();

    }
    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        if (UnifiedMenuController.isDeath)
        {
            gameObject.SetActive(false);
        }
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
        if (!other.CompareTag("Enemy"))
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
                speed = 30;
                break;
            case Difficulty.Normal:
                speed = 50;
                break;
            case Difficulty.Hard:
                speed = 80;
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