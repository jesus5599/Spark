using System.Collections;
using UnityEngine;

public class APUNTADO : MonoBehaviour
{
    public enum WeaponType { Pistol, Submachinegun, Argun }
    public WeaponType activeWeapon = WeaponType.Pistol;

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;

    public GameObject gun;
    public GameObject enemy;
    public Rigidbody misil;
    public Transform lanzador;
    private Transform seguir;
    public float rangeX = 6f;
    public float rangeY = 6f;
    public float rangeZ = 6f;

    public float firstShotDelay = 1f; // Retraso en el primer disparo al entrar en rango

    private bool hasEnteredRange = false; // Indica si el objetivo ha entrado en rango
    private bool canShoot = true;
    private bool isShooting = false;

    public float pistoldelay = 1f;
    public float submachinegundelay = 0.2f;
    public float argundelay = 0.5f;
    public float argunshootdelay, submachineshootdelay, pistolshootdelay;

    public AudioSource audioSource;
    public AudioClip ArClip, SubmachineClip, GunClip;
    public LayerMask Playerlayer;
    public bool survivalmode;
    private void OnEnable()
    {
        canShoot = true;
        isShooting = false;
        hasEnteredRange = false;
        LoadDifficulty();
        AdjustWeaponDelays();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        canShoot = true;
        isShooting = false;
        hasEnteredRange = false;
    }
    public void Start()
    {
        seguir = GameObject.FindWithTag("ObjetivoBala").transform;
    }
    void Update()
    {
        if (UnifiedMenuController.isDeath)
        {
            StopAllCoroutines();
            canShoot = true;
            isShooting = false;
            hasEnteredRange = false;
            return;
        }

        ShowRange();

        if (!gameObject.activeInHierarchy || !canShoot)
            return;

        if (IsTargetInRange())
        {
            AimAtTarget();

            // Si es la primera vez que entra en rango, esperar el primer disparo
            if (!hasEnteredRange)
            {
                hasEnteredRange = true;
                StartCoroutine(DelayedFirstShot());
                return;
            }

            // Si puede disparar, se detiene
            if (!IsObstacleInWay() && canShoot && !isShooting)
            {
                EnemyAI.shoot = true; // Detener movimiento
                switch (activeWeapon)
                {
                    case WeaponType.Pistol:
                        StartCoroutine(Pistolshot());
                        break;
                    case WeaponType.Submachinegun:
                        StartCoroutine(Submachineshot());
                        break;
                    case WeaponType.Argun:
                        StartCoroutine(ARshot());
                        break;
                }
            }
            else
            {
                // Si no puede disparar (obstáculo en el camino) y está en rango, sigue moviéndose hacia el objetivo
                EnemyAI.shoot = false; // No detenerse si no puede disparar
                
            }
        }
        else
        {
            hasEnteredRange = false; // Reiniciar estado si el objetivo sale del rango
            EnemyAI.shoot = false; // Permitir movimiento fuera del rango
            
        }
    }


    private bool IsTargetInRange()
    {
        return Mathf.Abs(transform.position.x - seguir.position.x) <= rangeX &&
               Mathf.Abs(transform.position.y - seguir.position.y) <= rangeY &&
               Mathf.Abs(transform.position.z - seguir.position.z) <= rangeZ;
    }

    private bool IsObstacleInWay()
    {
        int excludePlayerLayer = ~Playerlayer.value; // Invertir bitmask para excluir la capa específica
        RaycastHit hit;
        Vector3 direction = (seguir.position - lanzador.position).normalized;
        float distance = Vector3.Distance(lanzador.position, seguir.position);

        if (Physics.Raycast(lanzador.position, direction, out hit, distance+1, excludePlayerLayer))
        {
            if (hit.collider.gameObject != seguir.gameObject)
            {
                return true; // Hay un obstáculo en el camino
            }
        }
        return false;
    }

    private IEnumerator DelayedFirstShot()
    {
        yield return new WaitForSeconds(firstShotDelay);
        hasEnteredRange = true; // Permitir disparos después del retraso inicial
    }

    private void ShowRange()
    {
        Debug.DrawRay(transform.position, Vector3.right * rangeX, Color.red);
        Debug.DrawRay(transform.position, Vector3.up * rangeY, Color.green);
        Debug.DrawRay(transform.position, Vector3.forward * rangeZ, Color.blue);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(rangeX * 2, rangeY * 2, rangeZ * 2));
    }

    private void AimAtTarget()
    {
        Vector3 difference = seguir.position - transform.position;
        Vector3 lookDirection = new Vector3(difference.x, 0, difference.z);

        Quaternion rotationY = Quaternion.LookRotation(lookDirection);
        Quaternion rotation = Quaternion.LookRotation(difference);

        gun.transform.rotation = rotation;
        enemy.transform.rotation = Quaternion.Euler(0, rotationY.eulerAngles.y, 0);
    }

    private void LoadDifficulty()
    { if (!survivalmode)
        {
            if (PlayerPrefs.HasKey("Difficulty"))
            {
                int difficultyValue = PlayerPrefs.GetInt("Difficulty");
                currentDifficulty = (Difficulty)difficultyValue;
            }
            else
            {
                currentDifficulty = Difficulty.Normal;
            }
        }
      
        else
        {
            currentDifficulty = Difficulty.Easy;
        }
    }

    private void AdjustWeaponDelays()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                pistoldelay = 1.5f;
                submachinegundelay = 0.2f;
                argundelay = 0.2f;
                pistolshootdelay = 2f;
                submachineshootdelay = 3f;
                argunshootdelay = 2.5f;
                break;
            case Difficulty.Normal:
                pistoldelay = 1f;
                submachinegundelay = 0.1f;
                argundelay = 0.1f;
                pistolshootdelay = 1f;
                submachineshootdelay = 1f;
                argunshootdelay = 1f;
                break;
            case Difficulty.Hard:
                pistoldelay = 0.5f;
                submachinegundelay = 0.075f;
                argundelay = 0.075f;
                pistolshootdelay = 0.5f;
                submachineshootdelay = 0.5f;
                argunshootdelay = 0.75f;
                break;
        }
    }

    private IEnumerator Pistolshot()
    {
        isShooting = true;
        PlayAudio(GunClip);
        Instantiate(misil, lanzador.position, lanzador.rotation).gameObject.SetActive(true);
        yield return new WaitForSeconds(pistolshootdelay);
        isShooting = false;
    }

    private IEnumerator ARshot()
    {
        isShooting = true;
        for (int i = 0; i < 3; i++)
        {
            PlayAudio(ArClip);
            Instantiate(misil, lanzador.position, lanzador.rotation).gameObject.SetActive(true);
            yield return new WaitForSeconds(argundelay);
        }
        yield return new WaitForSeconds(argunshootdelay);
        isShooting = false;
    }

    private IEnumerator Submachineshot()
    {
        isShooting = true;
        for (int i = 0; i < 10; i++)
        {
            PlayAudio(SubmachineClip);
            Instantiate(misil, lanzador.position, lanzador.rotation).gameObject.SetActive(true);
            yield return new WaitForSeconds(submachinegundelay);
        }
        yield return new WaitForSeconds(submachineshootdelay);
        isShooting = false;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
        PlayerPrefs.SetInt("Difficulty", (int)newDifficulty);
        PlayerPrefs.Save();
    }

    public void StopCorrutine()
    {
        StopAllCoroutines();
    }
}
