using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VolumeController : MonoBehaviour
{
    [Header("Volume Slider")]
    public string sliderName = "Slider"; // Nombre del Slider en la escena
    private Slider volumeSlider;
    [SerializeField] private TMPro.TextMeshProUGUI volumeLabel; // Asignar en el Inspector
    [SerializeField] private AudioClip volumeChangeSound; // Efecto de sonido para cambio de volumen
    [SerializeField] private AudioSource uiAudioSource;  // AudioSource para efectos de interfaz

    public List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        // Cargar volumen guardado
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;

        // Buscar el Slider en la escena y configurarlo
        FindSlider();
        FindAllAudioSources(); // Buscar todas las fuentes de audio para sincronizar
    }

    private void FindSlider()
    {
        // Buscar el slider usando el Tag en lugar del nombre
        GameObject sliderObject = GameObject.FindWithTag("VolumeSliderTag");

        if (sliderObject != null)
        {
            volumeSlider = sliderObject.GetComponent<Slider>();

            if (volumeSlider != null)
            {
                // Configurar el valor inicial del slider
                volumeSlider.value = AudioListener.volume;

                // Suscribirse al evento de cambio de valor
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un Slider con el Tag 'VolumeSliderTag' en la escena.");
        }
    }

    private void FindAllAudioSources()
    {
        audioSources.Clear();
        AudioSource[] sources = FindObjectsOfType<AudioSource>();

        foreach (var source in sources)
        {
            audioSources.Add(source);
        }
    }

    public void SetVolume(float volume)
    {
        // Cambiar el volumen global
        AudioListener.volume = volume;

        // Sincronizar con todas las fuentes de audio
        foreach (var source in audioSources)
        {
            source.volume = volume;
        }

        // Reproducir un sonido cuando el volumen cambie
        PlayVolumeChangeSound();

        // Guardar el volumen
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();

        // Actualizar el texto del volumen (opcional)
        UpdateVolumeLabel(volume);
    }

    private void PlayVolumeChangeSound()
    {
        if (uiAudioSource != null && volumeChangeSound != null)
        {
            uiAudioSource.PlayOneShot(volumeChangeSound);
        }
    }

    private void UpdateVolumeLabel(float volume)
    {
        if (volumeLabel != null)
        {
            volumeLabel.text = $"{Mathf.RoundToInt(volume * 100)}%";
        }
    }

    // Si deseas un botón de reset
    public void ResetVolume()
    {
        SetVolume(1f); // Establece el volumen a 100%
        if (volumeSlider != null)
        {
            volumeSlider.value = 1f;
        }
    }
}
