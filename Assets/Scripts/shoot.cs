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
    IEnumerator ShowImpact()
    {
        GameObject impacto;
        impacto = Instantiate(particle, transform.position, transform.rotation);        
        impacto.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        Destroy(gameObject);
    }
    IEnumerator ShowFlash()
    {
        GameObject destello;
        destello = Instantiate(flash, transform.position, transform.rotation);
        destello.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ShowImpact());
       

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Parry"))
        {
            StartCoroutine(ShowImpact());
        }
            collision.GetComponent<Collider>().GetComponent<parry>()?.Shoot();
        

    }
    private void OnTriggerStay(Collider other)
    {
        Destroy(gameObject);
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