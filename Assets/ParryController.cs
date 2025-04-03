using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ParryController : MonoBehaviour
{
    [Header("Parry")]
    public GameObject parry;
    public LineRendererProgress progress;
    public float timeParry = 0.5f;
    public float parryCooldown = 1.0f;
    private bool isParrying = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip parryClip;

    [Header("Configuraci�n de Mano")]
    public bool isLeftHand;  // True si el parry est� en la mano izquierda

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor currentInteractor;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    public Controlador controlador; 

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Suscribirse a eventos de agarre
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        DetectarParry();
    }

    //  Detectar Parry
    void DetectarParry()
    {
        bool parryTrigger = isLeftHand ? controlador.Player.LeftParry.triggered : controlador.Player.RightParry.triggered;

        if (parryTrigger && isParrying)
        {
            StartCoroutine(Parry());
        }
    }

    IEnumerator Parry()
    {
        isParrying = false;
        parry.SetActive(true);

        if (audioSource && parryClip)
            audioSource.PlayOneShot(parryClip);

        LineRendererProgress.delay = timeParry / 13;
        progress.StartUnloading();
        yield return new WaitForSeconds(timeParry);

        parry.SetActive(false);
        LineRendererProgress.delay = parryCooldown / 13;
        progress.StartLoading();
        yield return new WaitForSeconds(parryCooldown);

        isParrying = true;
    }

    //  Eventos de Agarre (VR)
    void OnGrab(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }
}
