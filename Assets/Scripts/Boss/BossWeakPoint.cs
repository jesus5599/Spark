using UnityEngine;
using System.Collections;

public class BossWeakPoint : MonoBehaviour
{
    public BossController boss;
    public GameObject cableroto;
    public GameObject cableentero;
    public SphereCollider SphereCollider1;
    public LayerMask ExcludeLayer;
    public static bool inmune = false;
    public Material MatReposo, MatInmune;
    private static bool isCoroutineRunning = false; // Controla una única corrutina para todos

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    private float invencibilitytime;

    private void Start()
    {
        AdjustShootVelocity();
        LoadDifficulty();
        cableentero.SetActive(true);
        cableroto.SetActive(false);
        SphereCollider1 = GetComponent<SphereCollider>();
        StartCoroutine(InmuneGlobal());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Proyectil"))
        {
            cableentero.SetActive(false);
            cableroto.SetActive(true);

            // Deshabilitar el collider del objeto golpeado
            SphereCollider1.enabled = false;
            
            SphereCollider1.excludeLayers = ExcludeLayer;
            if (boss != null)
            {
                boss.WeakPointDestroyed();
            }

            // Iniciar inmunidad en todos los objetos
            if (!isCoroutineRunning)
            {
                StartCoroutine(InmuneGlobal());
            }
        }
    }

    IEnumerator InmuneGlobal()
    {
        isCoroutineRunning = true;
        inmune = true;

        // Obtener todos los objetos de la escena con este script
        BossWeakPoint[] weakPoints = FindObjectsOfType<BossWeakPoint>();

        // Deshabilitar colisiones y cambiar materiales en todos los objetos
        foreach (BossWeakPoint wp in weakPoints)
        {
            wp.SphereCollider1.enabled = false;
            if (wp.cableentero != null)
                wp.cableentero.GetComponent<Renderer>().material = MatInmune;
        }

        yield return new WaitForSeconds(invencibilitytime); // Mantener inmunidad fija por 5 segundos

        float tiempoEspera = 0.5f; // Inicia con un parpadeo lento

        for (int i = 0; i < 10; i++) // 10 parpadeos en total
        {
            foreach (BossWeakPoint wp in weakPoints)
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
        foreach (BossWeakPoint wp in weakPoints)
        {
            wp.SphereCollider1.enabled = true;
            if (wp.cableentero != null)
                wp.cableentero.GetComponent<Renderer>().material = MatReposo;
        }

        inmune = false;
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
                
                invencibilitytime = 2.71f;
               
                break;
            case Difficulty.Normal:
                
                invencibilitytime = 4.71f;
                
                break;
            case Difficulty.Hard:
                
                invencibilitytime = 6.71f;
                
                break;
        }
    }
    #endregion
}
