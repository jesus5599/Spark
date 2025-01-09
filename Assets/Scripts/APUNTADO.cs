using System.Collections;
using UnityEngine;

public class APUNTADO : MonoBehaviour
{
    public enum WeaponType { Pistol, Submachinegun, Argun }
    public WeaponType activeWeapon = WeaponType.Pistol;

    public GameObject gun;
    public GameObject enemy;
    public Rigidbody misil;
    public Transform lanzador;
    public Transform seguir;
    public float rangeX = 6f;
    public float rangeY = 6f;
    public float rangeZ = 6f;

    public float pistoldelay = 1f;
    public float submachinegundelay = 0.2f;
    public float argundelay = 0.5f;

    public AudioSource audioSource;
    public AudioClip ArClip, SubmachineClip, GunClip;
    [SerializeField]
    private bool canShoot = true;
    private bool isShooting = false;

    private void OnEnable()
    {
        canShoot = true; // Permitir disparos al activar el objeto
        isShooting = false;
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

    private IEnumerator Pistolshot()
    {
        isShooting = true;

        PlayAudio(GunClip);
        Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
        misilInstanc.gameObject.SetActive(true);

        yield return new WaitForSeconds(pistoldelay);

        isShooting = false;
    }

    private IEnumerator ARshot()
    {
        isShooting = true;

        PlayAudio(ArClip);
        for (int i = 0; i < 3; i++)
        {
            Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
            misilInstanc.gameObject.SetActive(true);
            yield return new WaitForSeconds(argundelay);
        }

        yield return new WaitForSeconds(argundelay * 10);
        isShooting = false;
    }

    private IEnumerator Submachineshot()
    {
        isShooting = true;

        PlayAudio(SubmachineClip);
        for (int i = 0; i < 10; i++)
        {
            Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
            misilInstanc.gameObject.SetActive(true);
            yield return new WaitForSeconds(submachinegundelay);
        }

        yield return new WaitForSeconds(submachinegundelay * 50);
        isShooting = false;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
