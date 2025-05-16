using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public Slider sensitivitySlider;
    public TMPro.TextMeshProUGUI sensitivityText;

    public float minSensitivity = 50f;  // Valor mínimo de sensibilidad
    public float maxSensitivity = 150f; // Valor máximo de sensibilidad

    void Start()
    {

        // Configuramos el slider de 1 a 10
        sensitivitySlider.minValue = 1;
        sensitivitySlider.maxValue = 10;
        sensitivitySlider.wholeNumbers = true;

        // Si no hay sensibilidad guardada, establecerla por defecto en visible = 5
        if (!PlayerPrefs.HasKey("SensitivityOffset"))
        {
            float defaultReal = MapVisibleToReal(5);
            SensitivityManager.SetSensitivityOffset(defaultReal);
        }

        // Cargar la sensibilidad real desde PlayerPrefs
        float realSensitivity = SensitivityManager.GetSensitivityOffset();
        float visibleValue = Mathf.RoundToInt(MapRealToVisible(realSensitivity));
        sensitivitySlider.value = Mathf.Clamp(visibleValue, 1, 10);

        sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);

        UpdateSensitivityText(visibleValue);
    }


    void Update()
    {
        UpdateSensitivityText(sensitivitySlider.value);
    }

    void OnSliderChanged(float visibleValue)
    {
        // Convertir el valor visible a la sensibilidad real
        float realSensitivity = MapVisibleToReal(visibleValue);
        SensitivityManager.SetSensitivityOffset(realSensitivity);
    }

    void UpdateSensitivityText(float visibleValue)
    {
        sensitivityText.text = "Sensitivity: " + visibleValue.ToString("F0");
    }

    // Convertir el valor visible de 1–10 a un valor de sensibilidad real en el rango min–max
    float MapVisibleToReal(float visible)
    {
        return minSensitivity + ((visible - 1f) / 9f) * (maxSensitivity - minSensitivity);
    }

    // Convertir el valor real de sensibilidad al valor visible de 1–10
    float MapRealToVisible(float real)
    {
        return 1f + ((real - minSensitivity) / (maxSensitivity - minSensitivity)) * 9f;
    }
}
