using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    #region Variables
    
    public GameObject trackingLaserPrefab;
    public GameObject missilePrefab;
    public GameObject explosionPrefab;
    public GameObject explosionPrefab2;
    public GameObject sweepingLaserPrefab;
    public GameObject chargingLaserPrefab;
    public Transform laserSpawnPointArriba, laserSpawnPointAbajo;
    public Transform missileSpawnPointArriba, missileSpawnPointAbajo;
    public Transform[] weakPoints;
    public Transform[] sweepingLaserPoints;

    private int weakPointsDestroyed = 0;
    private int phaseTwoHealth = 3;
    private bool isPhaseTwo = false;
    private bool isFuryMode = false;
    private float rotationSpeed = 75f;
    public AudioSource audioSource; // Componente AudioSource para reproducir sonido
    public AudioClip explosion,explosion2;
   

    public bool lasergiratorio = false;
    private float rotationDirection = 1f; // 1 para sentido horario, -1 para antihorario
    private float changeDirectionTime = 5f; // Cambia de dirección cada X segundos
    private float timer = 0f;

    public bool invencibility;
    public Material MatReposo, MatInmune;
    bool F2Atacs;

    public GameObject areapeq,areagran;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    private float invencibilitytime;

    float attackProbabilityLaser ;
    float attackProbabilityMisil ;
    float probabilityIncreaseRate ;

    public GameObject BossDoor;
    public GameObject musicfinal;

    public static int dañofase2 = 0;
    
    #endregion

    #region Start/Update
    void Start()
    {
        damageboss.inmune = true;
        AdjustShootVelocity();
        LoadDifficulty();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        usica.instancia.CambiarCancion(0); // Reemplaza con el índice de la canción
        StartCoroutine(AttackPatternPhaseOne());
    }
    

    private void Update()
    {
        if (lasergiratorio)
        {
            transform.Rotate(0, 0, rotationDirection * rotationSpeed * Time.deltaTime);

            // Contador para cambiar la dirección
            timer += Time.deltaTime;
            if (timer >= changeDirectionTime)
            {
                rotationDirection = (Random.Range(0, 2) == 0) ? 1f : -1f; // Cambia aleatoriamente
                timer = 0f; // Reiniciar el temporizador
            }
        }
    }
    #endregion

    #region Fase1
    // **Fase 1: Ataques desde el techo**
    IEnumerator AttackPatternPhaseOne()
    {
        yield return new WaitForSeconds(5f); 
        float attackProbabilityLaser1 = attackProbabilityLaser;
        float attackProbabilityMisil1 = attackProbabilityMisil;
        
        bool isLaserActive = false;

        while (!isPhaseTwo)
        {
            // Aumenta la probabilidad progresivamente hasta el 100%
            attackProbabilityLaser = Mathf.Min(attackProbabilityLaser + probabilityIncreaseRate * Time.deltaTime, 100f);
            attackProbabilityMisil = Mathf.Min(attackProbabilityMisil + probabilityIncreaseRate * Time.deltaTime, 100f);

            // Determina qué ataque ejecutar
            if (!isLaserActive && Random.Range(0f, 100f) <= attackProbabilityLaser)
            {
                isLaserActive = true; // Marca que el láser está activo
                StartCoroutine(FireTrackingLaser());
                attackProbabilityLaser = attackProbabilityLaser1; // Reinicia la probabilidad tras disparar
            }
            else if (Random.Range(0f, 100f) <= attackProbabilityMisil)
            {
                StartCoroutine(FireHomingMissile());
                attackProbabilityMisil = attackProbabilityMisil1; // Reinicia la probabilidad tras disparar
            }

            yield return new WaitForSeconds(1f); // Espera antes del próximo ciclo

            isLaserActive = false; // Permite que se dispare otro láser en el futuro
        }
    }


    #endregion
    #region Caer al suelo
    IEnumerator FallToGround()
    {
        areapeq.SetActive(true);
        audioSource.Stop();
        float fallSpeed = 15f;
        while (transform.position.y > 0) // Hasta que llegue al suelo
        {
            transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Asegurar que quede en el suelo

        GameObject explosionInstance = Instantiate(explosionPrefab, explosionPrefab.transform.position, Quaternion.identity); // Efecto de impacto
        audioSource.PlayOneShot(explosion);
        explosionInstance.SetActive(true);

        yield return new WaitForSeconds(2.9f);
        
        usica.instancia.CambiarCancion(1); // Reemplaza con el índice de la canción
        areapeq.SetActive(false);


    }

    #endregion
    #region Fase2
    // **Fase 2: Ataques cambiantes**
    IEnumerator AttackPatternPhaseTwo()
    {
        F2Atacs = true;
        yield return new WaitForSeconds(1f);
        while (isPhaseTwo)
        {
            if (phaseTwoHealth == 3)
            {
                yield return FireRotatingLasers();
                yield return new WaitForSeconds(2f);
            }
            else if (phaseTwoHealth == 2)
            {
                StartCoroutine(FireHomingMissiles());
                yield return FireRotatingLasers();
                yield return new WaitForSeconds(1.5f);
                rotationSpeed = rotationSpeed * 1.02f;
            }
           else if (phaseTwoHealth == 1 && !isFuryMode)
            {

                
                StartCoroutine(OverloadAttack());
            }
            else 
            {  
                { yield break; } // Detiene solo esta corutina sin afectar las demás
            }
        }
    }
    void StartPhaseTwo()
    {
        isPhaseTwo = true;
        // Simular la caída con una animación o moviéndolo poco a poco hacia abajo
        StartCoroutine(FallToGround());
        StartCoroutine(Invencibility());
    }
    #endregion
    #region Ataques

    // **Láser rastreador**
    IEnumerator FireTrackingLaser()
    { if (!isPhaseTwo)
        {
            GameObject laser = Instantiate(trackingLaserPrefab, laserSpawnPointAbajo.position, Quaternion.identity);
            laser.SetActive(true);
            TrackingLaser trackingScript = laser.GetComponent<TrackingLaser>();
        }
        else
        {
            GameObject laser = Instantiate(trackingLaserPrefab, laserSpawnPointArriba.position, Quaternion.identity);
            laser.SetActive(true);
            TrackingLaser trackingScript = laser.GetComponent<TrackingLaser>();
        }
        
        //trackingScript.ChargeLaser(1.5f);
        yield return new WaitForSeconds(1.5f);
    }

    // **Misil teledirigido**
    IEnumerator FireHomingMissile()
    { if (!isPhaseTwo)
        {
            GameObject missile = Instantiate(missilePrefab, missileSpawnPointAbajo.position, Quaternion.identity);
            missile.SetActive(true);
            HomingMissile missileScript = missile.GetComponent<HomingMissile>();
            missileScript.SetTarget(GameObject.FindGameObjectWithTag("ObjetivoBala").transform);
        }
        else
        {
            GameObject missile = Instantiate(missilePrefab, missileSpawnPointArriba.position, Quaternion.identity);
            missile.SetActive(true);
            HomingMissile missileScript = missile.GetComponent<HomingMissile>();
            missileScript.SetTarget(GameObject.FindGameObjectWithTag("ObjetivoBala").transform);
        }
            
        yield return new WaitForSeconds(0.5f);
    }

    // **Láser giratorio (solo gira mientras dispara)**
    IEnumerator FireRotatingLasers()
    {
        float rotationTime = 5f;
        float elapsedTime = 0f;
        foreach (Transform pos in sweepingLaserPoints)
        {
            GameObject laser = Instantiate(chargingLaserPrefab, pos.position, pos.rotation);
            laser.transform.SetParent(pos); // Hacer que la instancia sea hija de pos
            laser.SetActive(true);

        }
        yield return new WaitForSeconds(1.5f);
        lasergiratorio = true;
        while (elapsedTime < rotationTime)
        {
           
            foreach (Transform pos in sweepingLaserPoints)
            {
                GameObject laser = Instantiate(sweepingLaserPrefab, pos.position, pos.rotation);
                laser.transform.SetParent(pos); // Hacer que la instancia sea hija de pos
                laser.SetActive(true);
                
            }

            elapsedTime += 5f;
            yield return new WaitForSeconds(5f);
            
        }
        lasergiratorio = false;
        
    }
   


    // **Misiles en Fase 2**
    IEnumerator FireHomingMissiles()
    {
        int numeroFlotante = Random.Range(1, 4); 

        for (int i = 0; i < numeroFlotante; i++)
        {
            StartCoroutine(FireHomingMissile());
            yield return new WaitForSeconds(1f/numeroFlotante);
        }
        yield return new WaitForSeconds(1f*numeroFlotante);
    }

    // **Ataque de Sobrecarga**
    IEnumerator OverloadAttack()
    {      
        areagran.SetActive(true);
         isFuryMode = true;


       
        audioSource.Stop();
         yield return new WaitForSeconds(1f);
       
        GameObject explosionInstance = Instantiate(explosionPrefab2, explosionPrefab2.transform.position, Quaternion.identity);
         explosionInstance.SetActive(true);
        
         audioSource.PlayOneShot(explosion2);
        
         yield return new WaitForSeconds(2.9f);
        
        audioSource.Stop();
        usica.instancia.CambiarCancion(2); // Reemplaza con el índice de la canción
        areagran.SetActive(false);



        StartCoroutine(FuryAttackPattern());
    }

    #endregion
    #region Fury Mode
    // **Modo Furia**
    IEnumerator FuryAttackPattern()
    {
       
        while (isFuryMode)
        {
            StartCoroutine(FireTrackingLaser());
            StartCoroutine(FireHomingMissiles());
            yield return FireRotatingLasers();
            rotationSpeed = rotationSpeed * 1.05f;
            yield return new WaitForSeconds(1f);
            
        }
    }

    #endregion

    #region Daño
    // **Destruir los puntos débiles**
    public void WeakPointDestroyed()
    {
        BossWeakPoint.inmune=true;
        weakPointsDestroyed++;
        if (weakPointsDestroyed >= weakPoints.Length)
        {
            StartPhaseTwo();
        }
        
    }

    // **Recibe daño en Fase 2**
    public void TakeDamage()
    {
        if (!isPhaseTwo) return; // Solo recibe daño en la Fase 2
        if (invencibility) return; // No recibe daño 
        dañofase2++;
        
        phaseTwoHealth--;
        
        Debug.Log("¡El jefe ha sido golpeado! Vida restante: " + phaseTwoHealth);

        if (phaseTwoHealth <= 0)
        {
            Die();
            return;
        }
        Debug.Log("Invencibilidad activada" );
        StopCoroutine(Invencibility());
        StartCoroutine(Invencibility());
    }

    // **Muerte del jefe**
    void Die()
    {
       

        StopAllCoroutines();
        StartCoroutine(died());

    }
    
    IEnumerator died()
    {
        areapeq.SetActive(true);
        yield return new WaitForSeconds(1f);
        audioSource.Stop();
        GameObject explosionInstance = Instantiate(explosionPrefab, explosionPrefab.transform.position, Quaternion.identity); // Efecto de impacto
        audioSource.PlayOneShot(explosion);
        explosionInstance.SetActive(true);        
        Animator anim = BossDoor.GetComponent<Animator>();
        anim.enabled = true;
        areapeq.SetActive(false);

        GameObject music = Instantiate(musicfinal, musicfinal.transform.position, Quaternion.identity);
        music.SetActive(true);
        Destroy(gameObject);
        yield return null;
      
        

    }
    #endregion

    #region Invencibility
    IEnumerator Invencibility()
    {
        if (dañofase2 == 2)
        {
            invencibilitytime += 6;
        }
        invencibility = true;
        GetComponent<Renderer>().material = MatInmune;
        damageboss.inmune = true;
        yield return new WaitForSeconds(invencibilitytime); // Mantener inmunidad fija por 5 segundos

        float tiempoEspera = 0.5f; // Inicia con un parpadeo lento

        for (int i = 0; i < 10; i++) // 10 parpadeos en total
        {

            GetComponent<Renderer>().material = (i % 2 == 0) ? MatReposo : MatInmune;

            yield return new WaitForSeconds(tiempoEspera); // Espera el tiempo actual

            // Reducir progresivamente el tiempo de espera (se acelera)
            tiempoEspera *= 0.8f;

            // Límite mínimo de tiempo para evitar parpadeo instantáneo
            if (tiempoEspera < 0.1f)
            {
                tiempoEspera = 0.1f;
            }
        }
        GetComponent<Renderer>().material = MatReposo;
        if (isPhaseTwo && !F2Atacs)
        {
            StartCoroutine(AttackPatternPhaseTwo()); // Comenzar la Fase 2
        }
        Debug.Log("Invencibilidad Desactivada");
        invencibility = false;
        damageboss.inmune = false;
    }

    #endregion
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
                rotationSpeed = 55;
                invencibilitytime = 3.71f;
                 attackProbabilityLaser =30;
                attackProbabilityMisil = 15;
                probabilityIncreaseRate = 10;
                break;
            case Difficulty.Normal:
                rotationSpeed = 65;
                invencibilitytime = 4.71f;
                attackProbabilityLaser = 40;
                attackProbabilityMisil = 20;
                probabilityIncreaseRate = 15;
                break;
            case Difficulty.Hard:
                rotationSpeed = 75;
                invencibilitytime = 5.71f;
                attackProbabilityLaser = 50;
                attackProbabilityMisil = 25;
                probabilityIncreaseRate = 20;
                break;
        }
    }
    #endregion
}
