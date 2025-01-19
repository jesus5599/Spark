using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Necesario para usar Stack

public class UnifiedMenuController : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;       // Menú principal
    [SerializeField] private GameObject pauseMenu;      // Menú de pausa
    [SerializeField] private GameObject optionsMenu;    // Menú de opciones
    [SerializeField] private GameObject difficultyMenu; // Menú de selección de dificultad
    [SerializeField] private GameObject DeathMenu; // Menú de selección de dificultad
    public static bool isPaused = false;
    public static bool isDeath = false;// Estado del juego (pausado o no)
    [SerializeField] private float previousTimeScale = 1.0f;  // Guarda el tiempo anterior a pausar

    public Stack<GameObject> menuStack = new Stack<GameObject>(); // Pila para rastrear menús
    private PlayerInput playerInput;         // Referencia al sistema de entrada
    private Controlador controlador;
    private InputAction anyButtonAction;

    private void Start()
    {
        
         controlador = new Controlador();
        controlador.UI.Menu.performed += OnPause; // Suscribir el evento
        controlador.Enable(); // Habilitar entradas
        if (SceneManager.GetActiveScene().buildIndex == 0) // Verificar la escena inicial correctamente
        {
            ActivateMenu(mainMenu);
        }
        else
        {
            DeactivateMenu(mainMenu);
        }

        // Desactivar otros menús
        DeactivateMenu(pauseMenu);
        DeactivateMenu(optionsMenu);
        DeactivateMenu(difficultyMenu);
        DeactivateMenu(DeathMenu);
        Time.timeScale = 1f; // Asegurar que el tiempo comience normal
       

        // Suscribir al evento de carga de escena
        SceneManager.sceneLoaded += OnSceneLoaded;

        
       
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Desactivar todos los menús cuando se carga una nueva escena
        CloseAllMenus();
        Debug.Log("paquito");
        Time.timeScale = 1f;
        isPaused = false;
        // Verificar si es la escena principal y activar el menú adecuado
        if (scene.buildIndex == 0)
        {
            ActivateMenu(mainMenu);
            DeactivateMenu(pauseMenu);
            DeactivateMenu(optionsMenu);
            DeactivateMenu(difficultyMenu);
            DeactivateMenu(DeathMenu);
        }
        else { 
            DeactivateMenu(mainMenu);
            DeactivateMenu(pauseMenu);
            DeactivateMenu(optionsMenu);
            DeactivateMenu(difficultyMenu);
            DeactivateMenu(DeathMenu);
        }
    }

    public void Update()
    {

        Debug.Log("Scene Index: " + SceneManager.GetActiveScene().buildIndex);
        if (controlador != null && controlador.UI.Menu.WasPerformedThisFrame() )
        {
            OnPause(new InputAction.CallbackContext()); // Invocar pausa manualmente
        }
       


    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Cambiar estado de pausa
            if (isPaused)
            { if (SceneManager.GetActiveScene().buildIndex > 0) { ResumeGame(); } }

            else
            { if (SceneManager.GetActiveScene().buildIndex > 0) { PauseGame(); } }
               
        }
    }

    // Mostrar menú de pausa y detener el tiempo
    public void PauseGame()
    { if (!isDeath)
        {
            isPaused = true;

            // Guardar el estado actual del tiempo antes de pausar
            previousTimeScale = Time.timeScale;

            // Pausar el tiempo del juego
            Time.timeScale = 0f;

            // Activar el menú de pausa
            OpenMenu(pauseMenu);
        }
        
    }

    // Ocultar el menú de pausa y restaurar el tiempo
    public void ResumeGame()
    {
        if (!isDeath)
        {
            isPaused = false;

            // Restaurar el tiempo al valor previo a la pausa
            Time.timeScale = previousTimeScale;

            // Cerrar el menú de pausa
            CloseAllMenus();
        }
    }

    // Abrir un menú y guardar el actual como anterior
    public void OpenMenu(GameObject menuToOpen)
    {
        if (menuToOpen == null) return;

        // Si hay un menú abierto, desactivarlo
        if (menuStack.Count > 0)
        {
            GameObject currentMenu = menuStack.Peek();
            DeactivateMenu(currentMenu);
        }

        // Guardar el menú actual en la pila
        menuStack.Push(menuToOpen);
        ActivateMenu(menuToOpen);
    }

    // Cerrar el menú actual y volver al anterior
    public void CloseCurrentMenu()
    {
        if (menuStack.Count > 0)
        {
            // Cerrar el menú actual
            GameObject currentMenu = menuStack.Pop();
            DeactivateMenu(currentMenu);

            // Activar el menú anterior si existe
            if (menuStack.Count > 0)
            {
                GameObject previousMenu = menuStack.Peek();
                ActivateMenu(previousMenu);
            }
        }
    }

    // Cerrar todos los menús
    public void CloseAllMenus()
    {
        // Desactivar todos los menús
        while (menuStack.Count > 0)
        {
            GameObject currentMenu = menuStack.Pop();
            DeactivateMenu(currentMenu);
        }

        Debug.Log("Todos los menús han sido cerrados.");
    }

    // Navegar al menú de opciones
    public void OpenOptions()
    {
        OpenMenu(optionsMenu);
    }

    // Regresar al menú principal desde cualquier menú
    public void BackToMain()
    {
        SceneManager.LoadScene(0);
    }

    // Iniciar un nuevo juego (abre selección de dificultad)
    public void StartGame()
    {
        OpenMenu(difficultyMenu);
    }

    // Seleccionar dificultad y cargar la escena del juego
    public void SelectDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("Difficulty", difficulty); // Guardar dificultad seleccionada
        SceneManager.LoadScene("level1");          // Cargar la escena principal del juego
    }

    // Reiniciar el nivel actual
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo se reanude antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recargar la escena actual
    }

    // Salir del juego
    public void QuitGame()
    {
        Application.Quit(); // Salir del juego
        Debug.Log("El juego se cerrará (solo en compilación, no en el editor).");
    }

    // Activar un menú
    private void ActivateMenu(GameObject menu)
    {
        if (menu != null)
            menu.SetActive(true);
    }

    // Desactivar un menú
    private void DeactivateMenu(GameObject menu)
    {
        if (menu != null)
            menu.SetActive(false);
    }

    private void OnEnable()
    {
        controlador.Enable();

        

    }

    private void OnDisable()
    {
        controlador.Disable();
        

    }  
    public void Continue()
    {
        if (isDeath) 
        {
            Debug.Log("¡Cualquier botón ha sido presionado!");
            Controladorjugador DeathController = FindObjectOfType<Controladorjugador>();
            if (DeathController != null)
            {
                
                DeactivateMenu(mainMenu);
                DeactivateMenu(pauseMenu);
                DeactivateMenu(optionsMenu);
                DeactivateMenu(difficultyMenu);
                DeactivateMenu(DeathMenu);
                DeathController.respawn();
                isDeath = false;
            }
            else
            {
                Debug.LogError("No se encontró un objeto de tipo Controlador jugador en la escena.");
            }
        }
        
    }

    public void ShowDeathScreen()
    {
        isDeath = true;
        Debug.Log("Pantalla de muerte activada.");
        // Implementa la lógica para mostrar la DeathScreen
        OpenMenu(DeathMenu);
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
