using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;  // Importa el namespace para manejar escenas

public class VolumeController : MonoBehaviour
{
    [Header("Volume Slider")]
    public string sliderName = "Slider"; // Nombre del Slider en la escena
    private Slider volumeSlider;
    [SerializeField] private TMPro.TextMeshProUGUI volumeLabel; // Asignar en el Inspector
    [SerializeField] private AudioClip volumeChangeSound; // Efecto de sonido para cambio de volumen
    [SerializeField] private AudioSource uiAudioSource;  // AudioSource para efectos de interfaz
    private static VolumeController instance; // Instancia estática para evitar duplicados
    public List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        // Verificar si ya existe una instancia de VolumeController
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // No destruir este GameObject entre cargas de escenas
        }
        else
        {
            Destroy(this.gameObject); // Si ya existe una instancia, destruir este GameObject
            return;
        }

        // Suscribirse al evento cuando se carga una nueva escena
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Cargar volumen guardado
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;

        // Buscar el Slider en la escena y configurarlo
        FindSlider();
        EnsureUIAudioSourceInEmptySlot();
    }
    private void FixedUpdate()
    { //ASO NIA QUE PREGUNTAR A VORE SI ESTA BE AIXINA
        // Recorrer la lista de audioSources
        for (int i = 0; i < audioSources.Count; i++)
        {
            // Si encontramos un lugar vacío (null)
            if (audioSources[i] == null)
            {
                // Rellenamos con uiAudioSource
                audioSources[i] = uiAudioSource;
                break;  // Detenemos el ciclo después de encontrar el primer vacío
            }
        }
        
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CleanAudioSources();
        // Cada vez que se carga una nueva escena, buscar todas las fuentes de audio
        FindAllAudioSources();
        EnsureUIAudioSourceInEmptySlot();
    }
    private void EnsureUIAudioSourceInEmptySlot()
    {
        // Recorrer la lista de audioSources
        for (int i = 0; i < audioSources.Count; i++)
        {
            // Si encontramos un lugar vacío (null)
            if (audioSources[i] == null)
            {
                // Rellenamos con uiAudioSource
                audioSources[i] = uiAudioSource;
                break;  // Detenemos el ciclo después de encontrar el primer vacío
            }
        }
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
        // Limpiar la lista de audioSources antes de llenarla nuevamente
        audioSources.Clear();

        // Buscar todas las fuentes de audio en la escena actual
        AudioSource[] sources = FindObjectsOfType<AudioSource>();

        // Verificar si encontramos fuentes de audio
        if (sources.Length == 0)
        {
            Debug.LogWarning("No se encontraron fuentes de AudioSource en la escena.");
        }

        // Añadir solo las fuentes de audio que aún no están en la lista para evitar duplicados
        foreach (var source in sources)
        {
            // Verificar si ya existe en la lista (en caso de que ya esté agregado)
            if (!audioSources.Contains(source))
            {
                audioSources.Add(source);
            }
        }
       
    }
    private void CleanAudioSources()
    {
        // Eliminar cualquier fuente de audio que ya no esté activa
        audioSources.RemoveAll(source => source == null);
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

    private void OnDestroy()
    {
        // Asegurarse de desuscribirse del evento cuando el objeto sea destruido
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
