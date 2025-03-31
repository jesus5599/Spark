using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    private int sceneToLoad;
    private float targetProgress = 0f;
    public float fillSpeed = 1.5f; // 🔹 Incrementamos la velocidad para que la barra suba más rápido

    private bool isSceneReady = false;

    private void Start()
    {
        sceneToLoad = PlayerPrefs.GetInt("NextScene", 1);
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    public static void LoadScene(int sceneIndex)
    {
        PlayerPrefs.SetInt("NextScene", sceneIndex);

        SceneManager.LoadScene(5);
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            targetProgress = progress;

            if (operation.progress >= 0.9f)
            {
                isSceneReady = true;
                targetProgress = 1f; // La barra debe llegar al 100%
            }

            yield return null;
        }
    }

    private void FixedUpdate()
    {
        // 🔹 Aumentamos la velocidad de llenado de la barra
        progressBar.value = Mathf.Lerp(progressBar.value, targetProgress, fillSpeed * Time.deltaTime);
        progressText.text = "Loading... " + (progressBar.value * 100).ToString("F0") + "%";

        // 🔹 Cambia la escena apenas la barra llegue casi al 100% (más rápido)
        if (isSceneReady && progressBar.value >= 0.99f)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
