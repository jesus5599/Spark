using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject trackingLaserPrefab;
    public GameObject missilePrefab;
    public GameObject explosionPrefab;
    public GameObject explosionPrefab2;
    public GameObject sweepingLaserPrefab;
    public Transform laserSpawnPoint;
    public Transform missileSpawnPoint;
    public Transform[] weakPoints;
    public Transform[] sweepingLaserPoints;

    private int weakPointsDestroyed = 0;
    private int phaseTwoHealth = 3;
    private bool isPhaseTwo = false;
    private bool isFuryMode = false;
    private float rotationSpeed = 100f;
    public AudioSource audioSource; // Componente AudioSource para reproducir sonido
    public AudioClip music1,music2,music3,music4,explosion,explosion2;   
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        audioSource.PlayOneShot(music1);
        StartCoroutine(AttackPatternPhaseOne());
    }

    // **Fase 1: Ataques desde el techo**
    IEnumerator AttackPatternPhaseOne()
    {
        while (!isPhaseTwo)
        {
            StartCoroutine(FireTrackingLaser());
            yield return new WaitForSeconds(3f);
            StartCoroutine(FireHomingMissile());
            yield return new WaitForSeconds(4f);
        }
    }

    // **Fase 2: Ataques cambiantes**
    IEnumerator AttackPatternPhaseTwo()
    {
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

    // **Láser rastreador**
    IEnumerator FireTrackingLaser()
    {
        GameObject laser = Instantiate(trackingLaserPrefab, laserSpawnPoint.position, Quaternion.identity);
        laser.SetActive(true);
        TrackingLaser trackingScript = laser.GetComponent<TrackingLaser>();
        //trackingScript.ChargeLaser(1.5f);
        yield return new WaitForSeconds(1.5f);
    }

    // **Misil teledirigido**
    IEnumerator FireHomingMissile()
    {
        GameObject missile = Instantiate(missilePrefab, missileSpawnPoint.position, Quaternion.identity);
        missile.SetActive(true);
        HomingMissile missileScript = missile.GetComponent<HomingMissile>();
        missileScript.SetTarget(GameObject.FindGameObjectWithTag("ObjetivoBala").transform);
        yield return new WaitForSeconds(0.5f);
    }

    // **Láser giratorio (solo gira mientras dispara)**
    IEnumerator FireRotatingLasers()
    {
        float rotationTime = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            foreach (Transform pos in sweepingLaserPoints)
            {
                GameObject laser = Instantiate(sweepingLaserPrefab, pos.position, pos.rotation);
                laser.SetActive(true);
                Destroy(laser, 2f); // Destruir después de 2 segundos
            }
            elapsedTime += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }


    // **Misiles en Fase 2**
    IEnumerator FireHomingMissiles()
    {
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(FireHomingMissile());
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
    }

    // **Ataque de Sobrecarga**
    IEnumerator OverloadAttack()
    {
        
         isFuryMode = true;
        
         yield return null;
        
         audioSource.Stop();
        

         GameObject explosionInstance = Instantiate(explosionPrefab2, explosionPrefab2.transform.position, Quaternion.identity);
         explosionInstance.SetActive(true);
        
         audioSource.PlayOneShot(explosion2);
        
         yield return new WaitForSeconds(2.9f);
        
         audioSource.Stop();
         
         audioSource.PlayOneShot(music3);
        


         StartCoroutine(FuryAttackPattern());
    }


    // **Modo Furia**
    IEnumerator FuryAttackPattern()
    {
        while (isFuryMode)
        {
            StartCoroutine(FireHomingMissiles());
            yield return FireRotatingLasers();
            yield return FireSweepingLaser();
            yield return new WaitForSeconds(1f);
        }
    }

    // **Láser de Barrido**
    IEnumerator FireSweepingLaser()
    {
        GameObject sweepingLaser = Instantiate(sweepingLaserPrefab, laserSpawnPoint.position, Quaternion.identity);
        sweepingLaser.transform.rotation = Quaternion.Euler(0, 0, 90);
        yield return new WaitForSeconds(1.5f);
    }

    // **Destruir los puntos débiles**
    public void WeakPointDestroyed()
    {
        weakPointsDestroyed++;
        if (weakPointsDestroyed >= weakPoints.Length)
        {
            StartPhaseTwo();
        }
    }

    void StartPhaseTwo()
    {
        isPhaseTwo = true;
        // Simular la caída con una animación o moviéndolo poco a poco hacia abajo
        StartCoroutine(FallToGround());
       
    }
    IEnumerator FallToGround()
    {
        audioSource.Stop();
        float fallSpeed =15f;
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
        audioSource.Stop();
        audioSource.PlayOneShot(music2);
        StartCoroutine(AttackPatternPhaseTwo()); // Comenzar la Fase 2

    }
    // **Recibe daño en Fase 2**
    public void TakeDamage()
    {
        if (!isPhaseTwo) return; // Solo recibe daño en la Fase 2

        phaseTwoHealth--;

        Debug.Log("¡El jefe ha sido golpeado! Vida restante: " + phaseTwoHealth);

        if (phaseTwoHealth <= 0)
        {
            Die();
        }
    }


    // **Muerte del jefe**
    void Die()
    {   audioSource.Stop();
        GameObject explosionInstance = Instantiate(explosionPrefab, explosionPrefab.transform.position, Quaternion.identity); // Efecto de impacto
        audioSource.PlayOneShot(explosion);
        explosionInstance.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(died());

    }

    IEnumerator died()
    {

        yield return new WaitForSeconds(2.9f);
        audioSource.Stop();
        audioSource.PlayOneShot(music4);
        Destroy(gameObject);

    }
}
