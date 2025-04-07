using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class VolumeController : MonoBehaviour
{
    [Header("Volume Slider")]
    public string sliderName = "Slider";
    private Slider volumeSlider;
    [SerializeField] private TMPro.TextMeshProUGUI volumeLabel;
    [SerializeField] private AudioClip volumeChangeSound;
    [SerializeField] private AudioSource uiAudioSource;
    private static VolumeController instance;
    public List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Cargar y aplicar el volumen guardado
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        ApplyVolume(savedVolume);

        FindSlider();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CleanAudioSources();
        FindAllAudioSources();
        FindSlider();

        // Aplicar el volumen guardado a las nuevas fuentes de audio
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        ApplyVolume(savedVolume);
    }

    private void FindSlider()
    {
        GameObject sliderObject = GameObject.FindWithTag("VolumeSliderTag");
        if (sliderObject != null)
        {
            volumeSlider = sliderObject.GetComponent<Slider>();
            if (volumeSlider != null)
            {
                volumeSlider.value = AudioListener.volume;
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

        if (sources.Length == 0)
        {
            Debug.LogWarning("No se encontraron fuentes de AudioSource en la escena.");
        }

        foreach (var source in sources)
        {
            if (!audioSources.Contains(source))
            {
                audioSources.Add(source);
            }
        }
    }

    private void CleanAudioSources()
    {
        audioSources.RemoveAll(source => source == null);
    }

    private void ApplyVolume(float volume)
    {
        AudioListener.volume = volume;
        foreach (var source in audioSources)
        {
            if (source != null) source.volume = volume;
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
        }

        UpdateVolumeLabel(volume);
    }

    public void SetVolume(float volume)
    {
        ApplyVolume(volume);

        // Reproducir sonido de cambio de volumen
        if (uiAudioSource != null && volumeChangeSound != null)
        {
            uiAudioSource.PlayOneShot(volumeChangeSound);
        }

        // Guardar configuración de volumen
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    private void UpdateVolumeLabel(float volume)
    {
        if (volumeLabel != null)
        {
            volumeLabel.text = $"{Mathf.RoundToInt(volume * 100)}%";
        }
    }

    public void ResetVolume()
    {
        SetVolume(1f);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
