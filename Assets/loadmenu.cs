using System.Collections;
using UnityEngine;
using static UnifiedMenuController;

public class loadmenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadMenu());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(12f);
        LoadingScreenManager.LoadScene(0);
    }
}
