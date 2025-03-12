using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject trackingLaserPrefab;
    public GameObject missilePrefab;
    public GameObject explosionPrefab;
    public GameObject sweepingLaserPrefab;
    public Transform laserSpawnPoint;
    public Transform missileSpawnPoint;
    public Transform[] weakPoints;

    private int weakPointsDestroyed = 0;
    private int phaseTwoHealth = 3;
    private bool isPhaseTwo = false;
    private bool isFuryMode = false;
    private float rotationSpeed = 100f;

    void Start()
    {
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
        HomingMissile missileScript = missile.GetComponent<HomingMissile>();
        missileScript.SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(0.5f);
    }

    // **Láser giratorio (solo gira mientras dispara)**
    IEnumerator FireRotatingLasers()
    {
        float rotationTime = 3f;
        float elapsedTime = 0f;
        while (elapsedTime < rotationTime)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            foreach (Transform pos in weakPoints)
            {
                Instantiate(trackingLaserPrefab, pos.position, pos.rotation);
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
        }
        yield return new WaitForSeconds(1f);
    }

    // **Ataque de Sobrecarga**
    IEnumerator OverloadAttack()
    {
        isFuryMode = true;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
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
        float fallSpeed =15f;
        while (transform.position.y > 0) // Hasta que llegue al suelo
        {
            transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Asegurar que quede en el suelo

        Instantiate(explosionPrefab, transform.position, Quaternion.identity); // Efecto de impacto

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
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
