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
    public Transform seguir;
    public float rangeX = 6f;
    public float rangeY = 6f;
    public float rangeZ = 6f;

    // Valores predeterminados para los tiempos de disparo
    public float pistoldelay = 1f;
    public float submachinegundelay = 0.2f;
    public float argundelay = 0.5f;
    public float argunshootdelay,submachineshootdelay,pistolshootdelay;
    public AudioSource audioSource;
    public AudioClip ArClip, SubmachineClip, GunClip;
    [SerializeField]
    private bool canShoot = true;
    private bool isShooting = false;

    private void OnEnable()
    {
        canShoot = true; // Permitir disparos al activar el objeto
        isShooting = false;
        LoadDifficulty();  // Cargar la dificultad desde PlayerPrefs
        AdjustWeaponDelays(); // Ajustar los tiempos de disparo según la dificultad
    }

    private void OnDisable()
    {
        StopAllCoroutines(); // Detener todas las corrutinas en ejecución
        canShoot = true;     // Reiniciar el estado de disparo
        isShooting = false;
    }

    void Update()
    {
        // Salir si el objeto no está activo o no puede disparar
        if (!gameObject.activeInHierarchy || !canShoot)
            return;

        // Verificar si el objetivo está en rango
        if (IsTargetInRange())
        {
            AimAtTarget();

            // Disparar según el arma activa
            if (canShoot && !isShooting)
            {
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
        }
    }

    private bool IsTargetInRange()
    {
        return Mathf.Abs(transform.position.x - seguir.position.x) <= rangeX &&
               Mathf.Abs(transform.position.y - seguir.position.y) <= rangeY &&
               Mathf.Abs(transform.position.z - seguir.position.z) <= rangeZ;
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

    private void AdjustWeaponDelays()
    {
        // Ajusta los tiempos de disparo según la dificultad
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                pistoldelay = 1.5f; // Mayor delay en dificultad fácil
                submachinegundelay = 0.2f;
                argundelay = 0.2f;
                pistolshootdelay =2f;
                submachineshootdelay =3f;
                argunshootdelay =2.5f;
                break;
            case Difficulty.Normal:
                pistoldelay = 1f; // Delay estándar en dificultad normal
                submachinegundelay = 0.1f;
                argundelay = 0.1f;
                pistolshootdelay = 1f;
                submachineshootdelay = 1f;
                argunshootdelay = 1f;
                break;
            case Difficulty.Hard:
                pistoldelay = 0.5f; // Menor delay en dificultad difícil
                submachinegundelay = 0.075f;
                argundelay = 0.075f;
                pistolshootdelay = .5f;
                submachineshootdelay = .5f;
                argunshootdelay = .75f;
                break;
        }
    }

    private IEnumerator Pistolshot()
    {
        isShooting = true;

        PlayAudio(GunClip);
        Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
        misilInstanc.gameObject.SetActive(true);

        yield return new WaitForSeconds(pistolshootdelay);

        isShooting = false;
    }

    private IEnumerator ARshot()
    {
        isShooting = true;

        
        for (int i = 0; i < 3; i++)
        {
            PlayAudio(ArClip);
            Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
            misilInstanc.gameObject.SetActive(true);
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
            Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
            misilInstanc.gameObject.SetActive(true);
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

    // Para cambiar la dificultad y guardarla en PlayerPrefs
    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
        PlayerPrefs.SetInt("Difficulty", (int)newDifficulty);
        PlayerPrefs.Save();  // Guardar la dificultad en PlayerPrefs
    }
}
