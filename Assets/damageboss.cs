using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageboss : MonoBehaviour
{
    public BossController boss;
    public GameObject cableroto;
    public GameObject cableentero;
    public MeshCollider MeshCollider1;
    public LayerMask ExcludeLayer;
    public static bool inmune = true;
    public Material MatReposo, MatInmune;
    private static bool isCoroutineRunning = false; // Controla una única corrutina para todos
    public bool nimune=inmune;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    public float invencibilitytime;

    private void Start()
    {
        AdjustShootVelocity();
        LoadDifficulty();
        cableentero.SetActive(true);
        cableroto.SetActive(false);
        MeshCollider1 = GetComponent<MeshCollider>();
        
    }
    
    public void Update()
    { nimune = inmune;
    if (inmune) MeshCollider1.enabled = false;
    else MeshCollider1.enabled = true;
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Proyectil"))
        {

            if (!inmune)
            {  // Deshabilitar el collider del objeto golpeado
                MeshCollider1.enabled = false;

                if (boss != null)
                {
                    boss.TakeDamage();
                }
                MeshCollider1.excludeLayers = ExcludeLayer;
                // Iniciar inmunidad en todos los objetos
                if (!isCoroutineRunning)
                {
                    StartCoroutine(InmuneGlobal());
                }
                cableentero.SetActive(false);
                cableroto.SetActive(true);
            }
           
        }
    }
    public void Inmunidad()
    {
        
       
            Debug.Log("Inmunidad para todos");
        StopAllCoroutines();
        StartCoroutine(InmuneGlobal());
        
    }

    IEnumerator InmuneGlobal()
    {
        Debug.Log("Inmunidad global");
        isCoroutineRunning = true;
        inmune = true;

        // Obtener todos los objetos de la escena con este script
        damageboss[] weakPoints = FindObjectsOfType<damageboss>();

        // Deshabilitar colisiones y cambiar materiales en todos los objetos
        foreach (damageboss wp in weakPoints)
        {
            wp.MeshCollider1.enabled = false;
            if (wp.cableentero != null)
                wp.cableentero.GetComponent<Renderer>().material = MatInmune;
            if (BossController.dañofase2 == 2)
            {
                wp.invencibilitytime += 6;

            }
        }

        yield return new WaitForSeconds(invencibilitytime); // Mantener inmunidad fija por 5 segundos

        float tiempoEspera = 0.5f; // Inicia con un parpadeo lento

        for (int i = 0; i < 10; i++) // 10 parpadeos en total
        {
            foreach (damageboss wp in weakPoints)
            {
                if (wp.cableentero != null)
                {
                    wp.cableentero.GetComponent<Renderer>().material = (i % 2 == 0) ? MatReposo : MatInmune;
                }
            }

            yield return new WaitForSeconds(tiempoEspera); // Espera el tiempo actual

            // Reducir progresivamente el tiempo de espera (se acelera)
            tiempoEspera *= 0.8f;

            // Límite mínimo de tiempo para evitar parpadeo instantáneo
            if (tiempoEspera < 0.1f)
            {
                tiempoEspera = 0.1f;
            }
        }


        // Restaurar colisiones y materiales en todos los objetos
        foreach (damageboss wp in weakPoints)
        {
            wp.MeshCollider1.enabled = true;
            if (wp.cableentero != null)
                wp.cableentero.GetComponent<Renderer>().material = MatReposo;
           
            
        }

        
        isCoroutineRunning = false;
    }
    #region Dificult
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
    private void AdjustShootVelocity()
    {
        // Ajusta los tiempos de disparo según la dificultad
        switch (currentDifficulty)
        {
            case Difficulty.Easy:

                invencibilitytime = 3.71f;

                break;
            case Difficulty.Normal:

                invencibilitytime = 4.71f;

                break;
            case Difficulty.Hard:

                invencibilitytime = 5.71f;

                break;
        }
    }
    #endregion
}
