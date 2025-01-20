using UnityEngine;
using Cinemachine;
using static UnityEngine.UI.Image;
using System;
using UnityEngine.InputSystem.XR;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.Windows;
using UnityEngine.UIElements;
using System.Security.Cryptography;

public class Controladorjugador : MonoBehaviour
{
    #region Variables
    private const float referenceDpi = 160f; // DPI de referencia para ajustes
    // Componentes y referencias
    private CharacterController characterController;

    [SerializeField] private Vector3 playerVelocity;
    private Controlador controlador;

    // Configuración de movimiento básico
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    public bool groundedPlayer;
    
    // Configuración de wall run
    public float wallRunSpeed = 10f ;
    public float wallRunDuration = 1.5f;


    //configuracion walljump
    
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallJumpTime = .25f, wallJumpSpeed = 20f;
    bool wallLeft, wallRight;

    public bool isWallRunning = false;
    private Vector3 wallNormal;
    private Vector3 preWallRunVelocity; // Dirección antes de wall run
    private Vector3 entryDirection; // Dirección al entrar en el wall run
    private float wallRunTimer;

    // Configuración de detección de suelo
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;

    // Configuración de detección de la pared
    public float wallDetectionDistance = 1f;
    public float  salidarayos = .9f;
    public LayerMask wallLayer;
    RaycastHit hitLeft, hitRight;

    //Configuracion Disparo
    public float tiempodisparo, timeAux;

    //Configuracion parar el tiempo
    public float TimeCooldown,TimeSlowed;
    public bool timeslow;

    // Variables para el control de la cámara con el ratón
    public CinemachineVirtualCamera virtualCamera; // Referencia a la Cinemachine Virtual Camera
    public Transform playerBody; // Referencia al cuerpo del jugador (para moverlo horizontalmente)
    public float Sensitivity;
    public float SensitivityX = 2.0f; // Sensibilidad  en el eje X
    public float SensitivityY = 2.0f; // Sensibilidad en el eje Y

    private float xRotation = 0f; // Rotación en el eje X (vertical)
    public Transform Cabeza;

    //Configuracion de dash
    public float dashSpeed, dashCooldown;
    public bool dashEnable;
    public ParticleSystem DashParticles; // Sistema de partículas
    //Configuracion de las animaciones
    private Animator animate;
    public float speedx, speedz;
    bool paredright, paredleft;

    //Configuracion del deslizamiento
    public bool isSliding;         
    public float slideSpeed = 10f;                 // Velocidad del deslizamiento
    public float slideDuration = 1f;              // Duración del deslizamiento
    public float crouchHeight = 0.9f;             // Altura al agacharse
    public float originalHeight = 1.8f;             // Altura original                
    private float slideTimer = 0f;    
    private Vector3 slideDirection;
    public LayerMask ceilingLayer;                  // Para detectar techos
    public bool tocandotecho;
    public LayerMask slideLayer;                   // Para detectar rampas
    public float rampSlideSpeedMultiplier = 1.5f; // Velocidad adicional en rampas
    private bool isOnRamp = false;
    public float slideUpForce;
    public float floorSideForce;
    public float SlideJumpSpeed;
    public GameObject timelow;

    public AudioSource audioSource; // Componente AudioSource para reproducir sonido
    public AudioClip pasosClip;   // Sonido de correr
    bool run=true;

    public Vector3 checkpointposition;
    public Quaternion checkpointrotation;
    private int currentCheckpointID = 0; // ID del último checkpoint alcanzado
    private EnemyManager enemyManager; // Referencia al gestor de enemigos
    public float currentTime; // Tiempo actual del jugador
    private float checkpointTime; // Tiempo registrado en el checkpoint
    private bool isDead = false; // Estado del jugador
    [SerializeField] private TMPro.TextMeshProUGUI tiempopartida; // Asignar en el Inspector

    public GameObject parry;
    public float timeparry, parrycooldown;
    public bool Isparring,counter;
    public LayerMask ParryLayer;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;

    public LineRendererProgress progress;
    private PlayerInput playerInput; // Referencia al componente PlayerInput

    
    #endregion
    #region Awake Start Update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        controlador = new Controlador();
        virtualCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        dashEnable = true;
        timeAux = Time.unscaledTime;
        timeslow = true;
        animate = GetComponent<Animator>();
        isSliding = false;
        tocandotecho = false;
        checkpointposition= transform.position;
        checkpointrotation = transform.rotation;
        enemyManager = FindObjectOfType<EnemyManager>(); // Encuentra el gestor de enemigos
        
        Isparring =true;

    }
    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        // Busca un GameObject llamado "Texto" y obtiene el componente TextMeshProUGUI
        GameObject textObject = GameObject.Find("tiempo");

        if (textObject != null)
        {
            tiempopartida = textObject.GetComponent<TMPro.TextMeshProUGUI>();
            tiempopartida.text = "Texto actualizado!";
        }
        else
        {
            Debug.LogWarning("No se encontró un GameObject llamado 'tiempo'.");
        }
    }
    void Update()
    {
        if (tiempopartida != null)
        {
            // Convertir el tiempo total transcurrido a minutos y segundos
            int minutes = Mathf.FloorToInt(currentTime / 60); // Minutos enteros
            float seconds = currentTime % 60; // Segundos sobrantes

            // Formatear el texto como MM:SS.ss
            tiempopartida.text = $"Tiempo: {minutes:00}:{seconds:00.00}";
        }
        
        if (UnifiedMenuController.isPaused)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            return;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = true;
        }
        if (UnifiedMenuController.isDeath)
        {
            controlador.Disable();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            
            
            return;
        }
        else
        {
            
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = true;
        }


        if (controlador.Player.Sensitivity.ReadValue<Vector2>().x<-.5)
        {
            Sensitivity -= 0.001f;
        }
        if (controlador.Player.Sensitivity.ReadValue<Vector2>().x > .5)
        {
            Sensitivity += 0.001f;
        }
        cameraoffset();

        HandleAnimations();

        int excludeParryLayer = ~ParryLayer.value;
        Vector3 puntopantalla = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Ray rayo = Camera.main.ScreenPointToRay(puntopantalla);
        RaycastHit hit;

        // Manejar el Disparo
        if (controlador.Player.Shot.triggered && Time.unscaledTime - timeAux > tiempodisparo)
        {
            if (Physics.Raycast(rayo, out hit, 1000, excludeParryLayer))
            {
                disparo.puntoimpacto = hit.point;
                disparo.disparoarma = true;
                timeAux = Time.unscaledTime;
            }


        }
        // Mover la cámara 
        PlayerLook();

        // Verificar si el jugador está en el suelo
        CheckGroundStatus();

        // Aplicar movimiento y salto
        if (isWallRunning == false && isSliding == false && isOnRamp==false)
        {
            HandleMovement();
        }

        Jump();
        // Manejar wall running
        if (isWallRunning)
        {
            HandleWallRun();
        }
        else
        {
            CheckForWall();
        }
        // Manejar el Dash
        if (controlador.Player.Dash.triggered && dashEnable && !tocandotecho)
        {            
            StartCoroutine (Dash(Camera.main.transform.forward));
        }
        // Manejar el Tiempo  
        if (controlador.Player.SlowTime.triggered && timeslow)
        {
            StartCoroutine(TimeStop());
        }

        // Aplicar gravedad y mover el jugador
        ApplyGravity();
        characterController.Move(playerVelocity * Time.unscaledDeltaTime);

        isOnRamp = IsOnRamp();

        if (controlador.Player.CrouchSlide.triggered && groundedPlayer)
        {
            StartSlide();
        }
        if (isSliding && !groundedPlayer)
        {
            StopSlide();
        }
        // Deslizar mientras el temporizador esté activo
        if (isSliding)
        {
            Slide();
        }
        if (isOnRamp && !isSliding)
        {
            StartRampSlide();
        }
        // Manejar el Escudo 
        if (controlador.Player.Deflect.triggered && Isparring)
        {
            StartCoroutine(Parry());
        }
        //Tiempo
        if (!isDead)
        {
            currentTime += Time.unscaledDeltaTime;
        }

        if (playerVelocity.y < -100)
        { 
        playerVelocity.y = -100;
        }

    }
    #endregion
    #region Jump and movement
    private void Jump()
    {
        
        // Saltar si está en el suelo
        if (controlador.Player.Jump.triggered && groundedPlayer && !isOnRamp && !tocandotecho)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
        // Saltar si está en el muro
        if (controlador.Player.Jump.triggered && isWallRunning)
        {
            WallJump();
        }
        if (controlador.Player.Jump.triggered && isOnRamp)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, slideLayer))
            {
                Vector3 floorNormalL = hit.normal;
                Vector3 forceToApply = transform.up * slideUpForce + floorNormalL * floorSideForce;
                characterController.Move(forceToApply.normalized);
                playerVelocity.y += gravityValue * Time.unscaledDeltaTime;
                StartCoroutine(JumpOfRamp(forceToApply));              
            }            
        }
    }
    private void CheckGroundStatus()
    {
        int excludeGroundLayer = ~groundLayer.value;
        // Usar raycast para verificar si el jugador está en el suelo
        Vector3 origin = transform.position;
        Vector3 direction = -transform.up;
        Debug.DrawRay(origin, direction * groundCheckDistance, Color.green);

        if (Physics.Raycast(origin, direction, groundCheckDistance, ~groundLayer))
        {
            groundedPlayer = true;
        }
        else
        {
            groundedPlayer = false;
        }

        // Reiniciar velocidad vertical si está en el suelo
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
    }

    private void HandleMovement()
    {
        
        // Movimiento horizontal usando el sistema de entrada
        Vector2 input = controlador.Player.Move.ReadValue<Vector2>();
        ;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = virtualCamera.transform.TransformDirection(move);
        move.y = 0;
        move = (transform.forward * move.z + transform.right * move.x).normalized;
        speedx = input.y;
        speedz = input.x;
        characterController.Move(move * Time.unscaledDeltaTime * playerSpeed);

        if(speedx<0 && run==true && groundedPlayer==true
            || speedz<0 && run == true && groundedPlayer == true 
           || speedx > 0 && run == true && groundedPlayer == true 
           || speedz > 0 && run == true && groundedPlayer == true) 
            StartCoroutine(Runsound());
       
    }
    private void ApplyGravity()
    {
        if (!isWallRunning)
        {
            playerVelocity.y += gravityValue * Time.unscaledDeltaTime;
        }
    }

    private void PlayerLook()
    {
        
            SensitivityX = Sensitivity;
            SensitivityY = Sensitivity;
        
        

        // Leer entrada de movimiento
        Vector2 lookInput = controlador.Player.Look.ReadValue<Vector2>();
        float lookX = lookInput.x * SensitivityX * Time.unscaledDeltaTime;
        float lookY = lookInput.y * SensitivityY * Time.unscaledDeltaTime;
        //Debug.Log(lookInput.x + "  " + lookInput.y);

        // Rotación vertical (cámara y cabeza)
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -80f, 56f); // Limitar la rotación vertical
        Quaternion verticalRotation = Quaternion.Euler(xRotation, 0f, 0f);
        virtualCamera.transform.localRotation = verticalRotation;
        Cabeza.transform.localRotation = verticalRotation;

        // Rotación horizontal (cuerpo del jugador)
        playerBody.Rotate(Vector3.up * lookX);
    }


    #endregion
    
    IEnumerator Dash(Vector3 moveDir)       
    {
        dashEnable = false;               
        float startTime = Time.unscaledTime;
        DashParticles.gameObject.SetActive(true);
        while (Time.unscaledTime < startTime + wallJumpTime)
        {
            characterController.Move(moveDir * dashSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        DashParticles.gameObject.SetActive(false);
        yield return new WaitForSeconds(dashCooldown);
        dashEnable = true;
    }
  
    #region Wallrun
    private void CheckForWall()
    {
        int excludeWallLayer = ~wallLayer.value; // Invierte el bitmask para excluir la capa específica

        
        

        // Detectar paredes a los lados del jugador
        Vector3 positionray = new Vector3 (transform.position.x,transform.position.y+salidarayos,transform.position.z);
        Debug.DrawRay(positionray, -transform.right * wallDetectionDistance, Color.red);
        Debug.DrawRay(positionray, transform.right * wallDetectionDistance, Color.blue);
        wallLeft = Physics.Raycast(positionray, -transform.right, out hitLeft, wallDetectionDistance, excludeWallLayer);
        wallRight = Physics.Raycast(positionray, transform.right, out hitRight, wallDetectionDistance, excludeWallLayer);

        if (wallLeft || wallRight)
        {
            if (wallLeft) { paredleft = true; paredright = false; }
        else if (wallRight) { paredright = true; paredleft = false; }
            wallNormal = wallLeft ? hitLeft.normal : hitRight.normal;
            StartWallRun(wallNormal);

        }
    }

    private void StartWallRun(Vector3 wallNormal)
    {
        isWallRunning = true;
        wallRunTimer = wallRunDuration;

        // Guardar la dirección de movimiento previa
        preWallRunVelocity = playerVelocity;

        // Almacenar la dirección de entrada al wall run (la dirección de movimiento al momento de entrar)
        entryDirection = transform.forward;

        // Desactivar la gravedad temporalmente durante el wall run
        playerVelocity.y = 0; // Cancelar efecto de gravedad durante el wall run

        // Calcular la dirección del movimiento sobre la pared
        Vector3 wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized; // Movimiento paralelo a la pared

        // Ajustar la dirección para que coincida con la entrada inicial
        if (Vector3.Dot(wallRunDirection, entryDirection) < 0)
        {
            wallRunDirection = -wallRunDirection; // Asegurarse de que el movimiento sea en la misma dirección de entrada
        }

        // Establecer la velocidad del jugador en la dirección del wall run
        playerVelocity = wallRunDirection * wallRunSpeed ;
    }


    private void HandleWallRun()
    {
        wallRunTimer -= Time.unscaledDeltaTime;

        // Si el wall run termina o se salta, detenerlo
        if (wallRunTimer <= 0 || controlador.Player.Jump.triggered)
        {
            StopWallRun();
        }
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        playerVelocity = Vector3.zero;
        paredright = false; paredleft = false;
    }
    
    void WallJump()
    {
        isWallRunning = false;
        StopWallRun();
        Vector3 forceToApply;
        if (wallLeft)
        {
            Vector3 wallNormalL = wallLeft ? hitLeft.normal : hitRight.normal;
            forceToApply = transform.up * wallJumpUpForce + wallNormalL * wallJumpSideForce;
            characterController.Move(forceToApply.normalized);
            playerVelocity.y += gravityValue * Time.unscaledDeltaTime;
            StartCoroutine(JumpOfWall(forceToApply));
           
        }
        if (wallRight)
        {
            Vector3 wallNormalR = wallRight ? hitRight.normal : hitLeft.normal;
            forceToApply = transform.up * wallJumpUpForce + wallNormalR * wallJumpSideForce;
            characterController.Move(forceToApply.normalized);
            playerVelocity.y += -9.81f * Time.unscaledDeltaTime;
            StartCoroutine(JumpOfWall(forceToApply));
            
        }
    }
    IEnumerator JumpOfWall(Vector3 moveDir)
    {
        playerVelocity.y = 0;
        
        float startTime = Time.unscaledTime;

        while (Time.unscaledTime < startTime + wallJumpTime)
        {
            characterController.Move(moveDir * wallJumpSpeed * Time.unscaledDeltaTime);
            yield return null;
        }               
    }
    #endregion


    #region Timestop
    IEnumerator TimeStop() 
    {
        timeslow = false;
            SlowDownTime();
        yield return new WaitForSeconds(TimeSlowed*.2f);
        RestoreTime();
        yield return new WaitForSeconds(TimeCooldown);
        timeslow = true;
    }
    // Ralentiza el tiempo al 50% de su velocidad normal
    public void SlowDownTime()
    {
        Time.timeScale = 0.2f; // Tiempo a la mitad de velocidad
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Ajusta el fixedDeltaTime para mantener la física sincronizada
        timelow.gameObject.SetActive(true);
    }

    // Restaura el tiempo a la velocidad normal
    public void RestoreTime()
    {
        Time.timeScale = 1f; // Tiempo normal
        Time.fixedDeltaTime = 0.02f; // Restaurar el valor original
        timelow.gameObject.SetActive(false);
    }
    #endregion 
    
    #region Slide
    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        if (IsSomethingAbove())
        {
            tocandotecho = true;            
        }
        // Reducir la altura del CharacterController
        characterController.height = crouchHeight;
        characterController.center = new Vector3(0f, crouchHeight/2, 0f);

        // Capturar la dirección de movimiento actual
        slideDirection = transform.forward * slideSpeed;
    }

    void Slide()
    {
        if (slideTimer > 0)
        {
            if (IsSomethingAbove())
            {
                tocandotecho = true;
            }
            slideTimer -= Time.unscaledDeltaTime;

            // Aplicar el movimiento del deslizamiento
            characterController.Move(slideDirection * Time.unscaledDeltaTime);
        }
        else
        {
            // Intentar detener el deslizamiento
            TryStopSlide();
        }
    }
    void TryStopSlide()
    {
        // Verificar si hay algo encima
        if (IsSomethingAbove())
        {
            tocandotecho = true;
            // Si hay algo encima, continuar deslizando
            slideTimer = 0.2f; // Extender temporalmente el deslizamiento
        }
        else
        {
            
            StopSlide();
        }
    }
    void StopSlide()
    {
        tocandotecho = false;
        isSliding = false;

        // Restaurar la altura original del CharacterController
        characterController.height = originalHeight;
        characterController.center = new Vector3(0f, originalHeight/2, 0f);

        playerVelocity = new Vector3(0, playerVelocity.y, 0);
    }
    bool IsSomethingAbove()
    {
        // Comprobar si hay un objeto por encima
        //Vector3 top = transform.position + Vector3.up * (originalHeight / 2);
        //return Physics.CheckSphere(top, 0.1f, ceilingLayer);
        return Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit,2f, ceilingLayer);
        
    }
    void StartRampSlide()
    {
        isSliding = true;

        // Generar deslizamiento hacia abajo en rampas
        //slideDirection = Vector3.ProjectOnPlane(Vector3.down, GetRampNormal()) * slideSpeed * rampSlideSpeedMultiplier;
        
        playerVelocity = Vector3.ProjectOnPlane(Vector3.down, GetRampNormal()) * slideSpeed * rampSlideSpeedMultiplier;
       // playerVelocity.y = -10;

    }
    bool IsOnRamp()
    {
        // Verificar si el personaje está sobre una rampa
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, slideLayer))
        {
            return true;
        }
        return false;
    }

    Vector3 GetRampNormal()
    {
        // Obtener la normal de la rampa debajo del personaje
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, slideLayer))
        {
            return hit.normal;
        }
        return Vector3.up; // Valor predeterminado si no hay rampa
    }
    IEnumerator JumpOfRamp(Vector3 moveDir)
    {
        playerVelocity.y = 0;

        float startTime = Time.unscaledTime;

        while (Time.unscaledTime < startTime + wallJumpTime)
        {
            characterController.Move(moveDir * SlideJumpSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
    }
    #endregion

    #region Die
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger con: " + other.name);
        if (other.transform.CompareTag("checkpoint"))
        {
            // Actualiza la posición e ID del checkpoint
            checkpointposition = other.transform.position;
            checkpointrotation = other.transform.rotation;
            checkpointTime = currentTime;
            currentCheckpointID = other.GetComponent<Checkpoint>().checkpointID;
            other.gameObject.SetActive(false);
            Debug.Log("Checkpoint alcanzado: " + currentCheckpointID);
        }
        else if (other.transform.CompareTag("Enemy"))
        {
            Muerto();
        }
        else if(other.transform.CompareTag("finish"))
            {
            UnifiedMenuController menuController = FindObjectOfType<UnifiedMenuController>();
            SaveSystem sistemaguardadotiempo = FindObjectOfType<SaveSystem>();
            if (menuController != null)
            {
                sistemaguardadotiempo.SaveNewTime(currentTime);
                menuController.ShowNextLevelMenu();
            }
            else
            {
                Debug.LogError("No se encontró un objeto de tipo UnifiedMenuController en la escena.");
            }
            Time.timeScale = 0f;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisión con: " + collision.gameObject.name);
        
        if (collision.transform.CompareTag("Enemy"))
        {
            Muerto();
        }
    }

    public void Muerto()
    {
        StopAllCoroutines();
        progress.StopAllCoroutines();        
        timeslow = true;
        counter = false;
        Isparring = true;
        dashEnable = true;
        parry.gameObject.SetActive(false);
        timelow.gameObject.SetActive(false);
        DashParticles.gameObject.SetActive(false);
        progress.PointsToOrigin();
        Time.timeScale = 0;

        // Llamar a la pantalla de muerte
        UnifiedMenuController menuController = FindObjectOfType<UnifiedMenuController>();
        if (menuController != null)
        {
            menuController.ShowDeathScreen();
        }
        else
        {
            Debug.LogError("No se encontró un objeto de tipo UnifiedMenuController en la escena.");
        }
    }

    public void respawn()
    {
        if (characterController != null)
        {
            characterController.enabled = false; // Desactiva el CharacterController temporalmente
        }

        Debug.Log("Reapareciendo en: " + checkpointposition);
        transform.position = checkpointposition; // Mueve al jugador
        transform.rotation = checkpointrotation; // Orienta al jugador
        enemyManager.RespawnEnemies(currentCheckpointID);
        if (characterController != null)
        {
            characterController.enabled = true; // Reactiva el CharacterController
        }
        Time.timeScale = 1f;
        controlador.Enable();
    }

    #endregion
    public void cameraoffset()
    {
        if (isSliding)
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(0f, 0.66f, 0.17f);
        }
        else
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(0f, 1.62f, 0.17f);
        }
    }
    IEnumerator Runsound()
    {
        run = false;
        if (audioSource != null && pasosClip != null)
        {
            audioSource.PlayOneShot(pasosClip);
        }
        yield return new WaitForSeconds(.948f);
        run = true;
    }
    IEnumerator Parry()
    {  
        counter = true;
        Isparring = false;
        parry.gameObject.SetActive(true);
        LineRendererProgress.delay = timeparry/13;
        progress.StartUnloading();
        yield return new WaitForSeconds(timeparry);
        counter = false;
        parry.gameObject.SetActive(false);
        LineRendererProgress.delay = parrycooldown/13;
        progress.StartLoading();
        yield return new WaitForSeconds(parrycooldown);
        
        Isparring = true;
    }
    private void HandleAnimations()
    {
        animate.SetFloat("speedx", speedx);
        animate.SetFloat("speedz", speedz);
        animate.SetFloat("y", playerVelocity.y);
        animate.SetBool("ground", groundedPlayer);
        animate.SetBool("pared", isWallRunning);
        animate.SetBool("paredright", paredright);
        animate.SetBool("paredleft", paredleft);
        animate.SetBool("slide", isSliding);
        animate.SetBool("parry", counter);
    }

    private void LoadDifficulty()
    {
        // Cargar la dificultad desde PlayerPrefs. Si no se ha guardado, se asume dificultad Normal.
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            int difficultyValue = PlayerPrefs.GetInt("Difficulty");
            currentDifficulty = (Difficulty)difficultyValue;
        }
        else
        {
            currentDifficulty = Difficulty.Normal; // Valor por defecto
        }
    }
    private void AdjustHabilitiesDelays()
    {
        // Ajusta los tiempos de disparo según la dificultad
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                dashCooldown = 1;
                tiempodisparo = .25f;
                timeparry = 4;
                parrycooldown = 1;
                TimeSlowed = 8;
                TimeCooldown = 2;
                break;
            case Difficulty.Normal:
                dashCooldown = 2;
                tiempodisparo = 0.5f;
                timeparry = 2;
                parrycooldown = 2;
                TimeSlowed = 4;
                TimeCooldown = 4;
                break;
            case Difficulty.Hard:
                dashCooldown = 4;
                tiempodisparo = 1;
                timeparry = 1;
                parrycooldown = 4;
                TimeSlowed = 2;
                TimeCooldown = 8;
                break;
        }
    }
    private void OnEnable()
    {
        controlador.Enable();
        LoadDifficulty();  // Cargar la dificultad desde PlayerPrefs
        AdjustHabilitiesDelays();
    }

    private void OnDisable()
    {
        controlador.Disable();
    }
}
