using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class FocusButton : MonoBehaviour
{
    public Button boton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(boton.gameObject);
        
    }
    public void FocoMenuPrincipal()
    {
        
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            EventSystem.current.SetSelectedGameObject(boton.gameObject);
        }
    }

}
