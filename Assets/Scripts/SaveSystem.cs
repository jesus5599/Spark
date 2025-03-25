using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
[System.Serializable]
public class TimeEntry
{
    public float time;
}

[System.Serializable]
public class LevelTimeData
{
    public List<TimeEntry> bestTimes = new List<TimeEntry>();
}

[System.Serializable]
public class KeyValuePair
{
    public string key;
    public LevelTimeData value;
}

[System.Serializable]
public class AllLevelsTimeData
{
    public List<KeyValuePair> levelTimes = new List<KeyValuePair>();
}

public class SaveSystem : MonoBehaviour
{
    private const string SaveFileName = "level_best_times.json";
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    private AllLevelsTimeData allLevelsTimeData = new AllLevelsTimeData();
    private const int MaxBestTimes = 5;

    [SerializeField] private TextMeshProUGUI bestTimesText;
    [SerializeField] private TextMeshProUGUI bestTimesText1;
    [SerializeField] private string currentLevel = "level1";
    public bool survivemode;
    private void Start()
    {
        LoadTimes();
        UpdateBestTimesText();
    }
    public void Update()
    {
        string nombreescena = SceneManager.GetActiveScene().name;
        if (nombreescena == ("Survive"))
        {
            survivemode = true;
        }
        else
        {
            survivemode = false;
        }
    }
    public void SetCurrentLevel(string levelName)
    {
        currentLevel = levelName;
    }

    public void SaveNewTime(float newTime)
    {
        var existingEntry = allLevelsTimeData.levelTimes.FirstOrDefault(pair => pair.key == currentLevel);

        // Si el nivel no existe, añadir uno nuevo
        if (existingEntry == null)
        {
            existingEntry = new KeyValuePair { key = currentLevel, value = new LevelTimeData() };
            allLevelsTimeData.levelTimes.Add(existingEntry);
        }

        // Añadir el tiempo y ordenar
        if (!survivemode)
        {
            existingEntry.value.bestTimes.Add(new TimeEntry { time = newTime });
            existingEntry.value.bestTimes = existingEntry.value.bestTimes
                .OrderBy(entry => entry.time)
                .Take(MaxBestTimes)
                .ToList();
        }
        else 
        {
            existingEntry.value.bestTimes.Add(new TimeEntry { time = newTime });
            existingEntry.value.bestTimes = existingEntry.value.bestTimes
                .OrderByDescending(entry => entry.time)
                .Take(MaxBestTimes)
                .ToList();

        }
        SaveTimes();
        UpdateBestTimesText();
    }

    private void SaveTimes()
    {
        if (allLevelsTimeData != null)
        {
            try
            {
                string json = JsonUtility.ToJson(allLevelsTimeData, true);
                string encodedJson = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
                File.WriteAllText(SaveFilePath, encodedJson);
                Debug.Log($"Datos guardados correctamente en {SaveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al guardar los datos: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("No hay datos para guardar.");
        }
    }


    private void LoadTimes()
    {
        if (File.Exists(SaveFilePath))
        {
            string encodedJson = File.ReadAllText(SaveFilePath);

            // Validar si el archivo no está vacío
            if (!string.IsNullOrEmpty(encodedJson))
            {
                try
                {
                    // Intentar decodificar el contenido Base64
                    string decodedJson = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedJson));
                    // Intentar deserializar el JSON
                    allLevelsTimeData = JsonUtility.FromJson<AllLevelsTimeData>(decodedJson);

                    Debug.Log("Datos cargados y decodificados con éxito.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error al decodificar o cargar JSON: {e.Message}");
                    // Si hay un error, inicializar con datos predeterminados
                    allLevelsTimeData = new AllLevelsTimeData();
                    CreateDefaultSaveFile(); // Crea un archivo válido
                }
            }
            else
            {
                Debug.LogWarning("El archivo de datos está vacío. Inicializando datos predeterminados.");
                allLevelsTimeData = new AllLevelsTimeData();
                CreateDefaultSaveFile(); // Crea un archivo válido
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el archivo. Inicializando datos predeterminados.");
            allLevelsTimeData = new AllLevelsTimeData();
            CreateDefaultSaveFile(); // Crea un archivo válido
        }
    }



    private void UpdateBestTimesText()
    {
        var existingEntry = allLevelsTimeData.levelTimes.FirstOrDefault(pair => pair.key == currentLevel);

        if (existingEntry != null && existingEntry.value.bestTimes.Count > 0)
        {
            bestTimesText.text = $"Top 5 Times for {currentLevel}:\n";
            bestTimesText1.text = $"Top 5 Times for {currentLevel}:\n";
            int rank = 1;
            foreach (var entry in existingEntry.value.bestTimes)
            {
                string formattedTime = FormatTime(entry.time);
                bestTimesText.text += $"{rank}. {formattedTime}\n";
                bestTimesText1.text += $"{rank}. {formattedTime}\n";
                rank++;
            }
        }
        else
        {
            bestTimesText.text = $"Top 5 Times for {currentLevel}:\nNo Data Yet";
            bestTimesText1.text = $"Top 5 Times for {currentLevel}:\nNo Data Yet";
        }
    }

    private void CreateDefaultSaveFile()
    {
        allLevelsTimeData = new AllLevelsTimeData();
        SaveTimes();
        Debug.Log("Archivo predeterminado creado.");
    }
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60); // Minutos
        int seconds = Mathf.FloorToInt(timeInSeconds % 60); // Segundos
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000) % 1000); // Milisegundos

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

}
