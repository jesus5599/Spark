using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GunController : MonoBehaviour
{
    public GameObject targetObject;  // Objetivo a apuntar
    public GameObject brazo;
    public Rigidbody misil;  // Prefab del misil
    public Transform lanzador;  // Punto de lanzamiento del misil
    public float veldisparo, tiempoDeRecarga;

    public int municioninicial = 6;
    private int municionactual;
    private bool recarga = true;

    public GameObject bala1, bala2, bala3, bala4, bala5, bala6;
    public AudioSource audioSource;
    public AudioClip disparoClip, recargaClip;
    public GameObject flash;

    private XRBaseInteractor currentInteractor;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        municionactual = municioninicial;

        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        UpdateBulletVisibility();
    }

    void Update()
    {
        if (municionactual <= 0 && recarga)
        {
            StartCoroutine(Reload());
            recarga = false;
        }

        if (currentInteractor != null)
        {
            XRController controller = currentInteractor.GetComponent<XRController>();

            if (controller != null)
            {
                controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isTriggerPressed);

                if (isTriggerPressed && municionactual > 0)
                {
                    StartCoroutine(ShowFlash());
                    Shoot();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (targetObject == null) return;

        Vector3 difference = targetObject.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(difference);
        brazo.transform.rotation = rotation;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as XRBaseInteractor;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    private void Shoot()
    {
        if (audioSource != null && disparoClip != null)
        {
            audioSource.PlayOneShot(disparoClip);
        }

        municionactual--;
        UpdateBulletVisibility();

        Rigidbody misilInstanc = Instantiate(misil, lanzador.position, lanzador.rotation);
        misilInstanc.transform.LookAt(targetObject.transform.position);
        misilInstanc.linearVelocity = lanzador.forward * veldisparo;
        misilInstanc.gameObject.SetActive(true);
    }

    private void UpdateBulletVisibility()
    {
        bala1.SetActive(municionactual >= 6);
        bala2.SetActive(municionactual >= 5);
        bala3.SetActive(municionactual >= 4);
        bala4.SetActive(municionactual >= 3);
        bala5.SetActive(municionactual >= 2);
        bala6.SetActive(municionactual >= 1);
    }

    IEnumerator Reload()
    {
        if (audioSource != null && recargaClip != null)
        {
            audioSource.PlayOneShot(recargaClip);
        }

        yield return new WaitForSeconds(tiempoDeRecarga);
        municionactual = municioninicial;
        UpdateBulletVisibility();
        recarga = true;
    }

    IEnumerator ShowFlash()
    {
        GameObject destello = Instantiate(flash, lanzador.position, lanzador.rotation);
        destello.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        Destroy(destello);
    }
}
