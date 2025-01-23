using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Necesario para usar Stack
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using static UnityEngine.UI.Image;
using System.Text;
using System;
using System.IO;
using UnityEngine.SocialPlatforms.Impl;


public class UnifiedMenuController : MonoBehaviour
{
    [System.Serializable]
    public class GameData
    {
        public int currentLevel; // Nivel actual
        public int score;        // Puntaje acumulado
    }

    private static string SaveFilePath => Application.persistentDataPath + "/savefile.dat"; // Cambié la extensión por .dat para mayor claridad
    private GameData gameData;
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;       // Menú principal
    [SerializeField] private GameObject pauseMenu;      // Menú de pausa
    [SerializeField] private GameObject optionsMenu;    // Menú de opciones
    [SerializeField] private GameObject difficultyMenu; // Menú de selección de dificultad
    [SerializeField] private GameObject DeathMenu; // Menú de muerte
    [SerializeField] private GameObject LevelFinishMenu; // Menú de siguiente nivel
    [SerializeReference] public static bool isPaused = false;
    [SerializeReference] public static bool isDeath = false;// Estado del juego (pausado o no)
    [SerializeReference] public static bool isWin = false;// Estado del juego (pausado o no)
    public bool paused, death, win;
    [SerializeField] private float previousTimeScale = 1.0f;  // Guarda el tiempo anterior a pausar

    public Stack<GameObject> menuStack = new Stack<GameObject>(); // Pila para rastrear menús
    private PlayerInput playerInput;         // Referencia al sistema de entrada
    private Controlador controlador;
    private InputAction anyButtonAction;
    public TextMeshProUGUI textMeshPro;
    // Opcional: Desactivar el botón de "Continuar" si no hay partida guardada
    public UnityEngine.UI.Button continueButton; // Referencia al botón de "Continuar"
   


    private void Start()
    {
        UpdateContinueButton();
        // Intentar cargar los datos guardados al iniciar el juego
        gameData = LoadGame() ?? new GameData { currentLevel = 1, score = 0 };
        isPaused = false;
        SetTransparency(true); // Hacer transparente
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
        DeactivateMenu(LevelFinishMenu);
        Time.timeScale = 1f; // Asegurar que el tiempo comience normal
       

        // Suscribir al evento de carga de escena
        SceneManager.sceneLoaded += OnSceneLoaded;

        
       
    }
    public void Update()
    {
        UpdateContinueButton();
        string nombreescena = SceneManager.GetActiveScene().name;
        SaveSystem guardado = FindObjectOfType<SaveSystem>();
        guardado.SetCurrentLevel(nombreescena);
        paused = isPaused;
        death = isDeath;
        win = isWin;
        Debug.Log("Scene Index: " + SceneManager.GetActiveScene().buildIndex);
        if (controlador != null && controlador.UI.Menu.WasPerformedThisFrame() )
        {
            OnPause(new InputAction.CallbackContext()); // Invocar pausa manualmente
        }
       


    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // Desactivar todos los menús cuando se carga una nueva escena
        CloseAllMenus();
        Debug.Log("paquito");
        Time.timeScale = 1f;
        isPaused = false;
        isDeath = false;
        isWin = false;
        SetTransparency(true); // Hacer transparente

        // Verificar si es la escena principal y activar el menú adecuado
        if (scene.buildIndex == 0)
        {
            ActivateMenu(mainMenu);
            DeactivateMenu(pauseMenu);
            DeactivateMenu(optionsMenu);
            DeactivateMenu(difficultyMenu);
            DeactivateMenu(DeathMenu);
            DeactivateMenu(LevelFinishMenu);
        }
        else
        {
            DeactivateMenu(mainMenu);
            DeactivateMenu(pauseMenu);
            DeactivateMenu(optionsMenu);
            DeactivateMenu(difficultyMenu);
            DeactivateMenu(DeathMenu);
            DeactivateMenu(LevelFinishMenu);
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
    { if (!isDeath && !isWin)
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
        if (!isDeath && !isWin)
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
        gameData = new GameData { currentLevel = 1, score = 0 }; // Reiniciar datos
        SaveGame(gameData);                                      // Guardar nueva partida
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

    public void ShowNextLevelMenu(int score)
    {   
        isWin = true;
        isPaused = true;
        gameData.currentLevel += 1; // Avanzar al siguiente nivel
        gameData.score += score;    // Actualizar puntaje
        SaveGame(gameData);         // Guardar progreso

        ActivateMenu(LevelFinishMenu);
        SetTransparency(false); // Hacer visible
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



    public void SetTransparency(bool isTransparent)
    {
        if (textMeshPro == null)
        {
            Debug.LogWarning("No se ha asignado un componente TextMeshProUGUI.");
            return;
        }

        // Establecer el color según si debe ser transparente o opaco
        Color currentColor = textMeshPro.color;
        currentColor.a = isTransparent ? 0f : 1f;  // 0f para transparente, 1f para opaco
        textMeshPro.color = currentColor;  // Asignamos el nuevo color al componente
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
   
    // Método para pasar al siguiente nivel y guardar el progreso
    public void LevelCompleted(int score)
    {
        
        // Cargar el siguiente nivel
        SceneManager.LoadScene("level" + gameData.currentLevel);
        Time.timeScale = 1f;
    }

    // Guardar los datos en un archivo cifrado en Base64
    private void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data); // Convertir datos a JSON
        string encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(json)); // Codificar en Base64
        File.WriteAllText(SaveFilePath, encryptedData); // Guardar en archivo
        Debug.Log("Partida guardada encriptada.");
    }

    // Cargar los datos desde un archivo cifrado en Base64
    private GameData LoadGame()
    {
        if (File.Exists(SaveFilePath))
        {
            string encryptedData = File.ReadAllText(SaveFilePath); // Leer archivo
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData)); // Decodificar Base64
            Debug.Log("Partida cargada y desencriptada.");
            return JsonUtility.FromJson<GameData>(json); // Convertir JSON a objeto
        }
        return null; // Si no hay datos guardados
    }

    // Método para continuar la partida desde el nivel guardado
    public void ContinueGame()
    {
        if (gameData != null && gameData.currentLevel > 1)
        {
            SceneManager.LoadScene("level" + gameData.currentLevel); // Cargar nivel guardado
        }
        else
        {
            Debug.Log("No hay una partida guardada.");
        }
    }
    // Actualizar la interactividad del botón según si hay partida guardada
    // Actualizar la interactividad del botón según los datos guardados
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            bool saveExists = File.Exists(SaveFilePath);

            if (saveExists)
            {
                // Cargar datos del archivo y verificar el nivel
                string encryptedData = File.ReadAllText(SaveFilePath);
                string jsonData = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
                GameData gameData = JsonUtility.FromJson<GameData>(jsonData);

                // El botón es interactuable solo si el nivel guardado es mayor que 1
                continueButton.interactable = gameData.currentLevel > 1;

                if (gameData.currentLevel == 1)
                {
                    Debug.Log("No hay progreso más allá del nivel 1. El botón de 'Continuar' está desactivado.");
                }
            }
            else
            {
                // No hay archivo de guardado, el botón no es interactuable
                continueButton.interactable = false;
                Debug.Log("No hay partida guardada.");
            }
        }
    }







}


