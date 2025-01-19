using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    public float speed;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;

    private void Start()
    {
        StartCoroutine(Destroy());
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

    private void OnCollisionEnter(Collision collision)
    {
        
        Destroy(gameObject);

    }
    private void OnTriggerEnter(Collider collision)
    {
collision.GetComponent<Collider>().GetComponent<parry>()?.Shoot();
        Destroy(gameObject);

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