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
using UnityEngine.SceneManagement;
public class ControladorjugadorVR : MonoBehaviour
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
    public float wallRunSpeed = 10f;
    public float wallRunDuration = 1.5f;
    // Radio del SphereCast
    public float sphereRadius = 0.5f;

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
    public float salidarayos = .9f;
    public LayerMask wallLayer;
    RaycastHit hitLeft, hitRight;

    //Configuracion Disparo
    public float tiempodisparo, timeAux;

    //Configuracion parar el tiempo
    public float TimeCooldown, TimeSlowed;
    public bool timeslow;
    public Light TimeLight;
    float targetIntensitytime = 4f; // Intensidad máxima de la luz
    Color originalColortime;

    // Variables para el control de la cámara con el ratón
    public CinemachineVirtualCamera virtualCamera; // Referencia a la Cinemachine Virtual Camera
    public Vector3 alturacamara, alturacamaraslide;
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
    public Light DashLight;
    float targetIntensitydash = 4f; // Intensidad máxima de la luz
    Color originalColordash;

    //Configuracion de las animaciones
    private Animator animate;
    public float speedx, speedz;
    bool paredright, paredleft;
    public float Animationtime;

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
    public bool run = true;

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
    public bool Isparring, counter;
    public LayerMask ParryLayer;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;

    public LineRendererProgress progress;
    public ControladorPuertasYBotones botones;
    private PlayerInput playerInput; // Referencia al componente PlayerInput

    private Vector3 lastPlatformPosition;
    private Transform currentPlatform = null;

    public bool survivalmode;

    public bool vr;
    public GameObject puntapistola;

    public GameObject CinematicCamera; // Referencia a la Cinemachine Virtual Camera
    public GameObject muñeco;

    public GameObject mesh;
    public CapsuleCollider capsuleCollider;
    bool finish;
    #endregion
    #region Awake Start Update
    void Awake()
    {
        finish = false;
        run = true;
        Animationtime = 1;
        targetIntensitytime = TimeLight.intensity;
        originalColortime = TimeLight.color;
        targetIntensitydash = DashLight.intensity;
        originalColordash = DashLight.color;
        characterController = GetComponent<CharacterController>();
        controlador = new Controlador();
        virtualCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        dashEnable = true;
        timeAux = Time.unscaledTime;
        timeslow = true;
        animate = GetComponent<Animator>();
        isSliding = false;
        tocandotecho = false;
        checkpointposition = transform.position;
        checkpointrotation = transform.rotation;
        enemyManager = FindObjectOfType<EnemyManager>(); // Encuentra el gestor de enemigos
        capsuleCollider = GetComponent<CapsuleCollider>();
        Isparring = true;
        if (botones == null)
        {
            botones = FindObjectOfType<ControladorPuertasYBotones>(); // Buscar en la escena

            if (botones == null) // Si no se encuentra en la escena
            {
                Debug.LogWarning("No se encontró el componente 'ControladorPuertasYBotones' en la escena.");
                // Si no se encuentra, añadirlo al objeto actual
                botones = gameObject.AddComponent<ControladorPuertasYBotones>();
                Debug.Log("Se ha añadido el componente 'ControladorPuertasYBotones' automáticamente.");
            }
            else
            {
                Debug.Log("Se ha encontrado el componente 'ControladorPuertasYBotones' en la escena.");
            }
        }

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
        if (currentPlatform != null)
        {
            // Calcula cuánto se ha movido la plataforma y mueve el personaje con ella
            Vector3 deltaMovement = currentPlatform.position - lastPlatformPosition;
            characterController.Move(deltaMovement);

            // Actualiza la posición anterior de la plataforma
            lastPlatformPosition = currentPlatform.position;
        }
        if (tiempopartida != null)
        {
            // Convertir el tiempo total transcurrido a minutos y segundos
            int minutes = Mathf.FloorToInt(currentTime / 60); // Minutos enteros
            float seconds = currentTime % 60; // Segundos sobrantes

            // Formatear el texto como MM:SS.ss
            tiempopartida.text = $"Time: {minutes:00}:{seconds:00.00}";
        }

        if (UnifiedMenuController.isPaused)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            Debug.Log("raton desbloqueado");
            return;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            Debug.Log("raton bloqueado");
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
            UnityEngine.Cursor.visible = false;
        }


        if (controlador.Player.Sensitivity.ReadValue<Vector2>().x < -.5)
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
        RaycastHit hit;
        Ray rayo;

        // Verificar si es VR o no y definir el rayo correspondiente
        if (vr)
        {
            Transform puntoDisparo = puntapistola.transform; // Asegurar que puntapistola está asignado
            rayo = new Ray(puntoDisparo.position, puntoDisparo.forward);

        }
        else
        {
            Vector3 puntopantalla = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
            rayo = Camera.main.ScreenPointToRay(puntopantalla);

        }
        

        // Manejar el Disparo
        if (controlador.Player.Shot.triggered && Time.unscaledTime - timeAux > tiempodisparo)
        {
            if (Physics.Raycast(rayo, out hit, 1000, excludeParryLayer))
            {
                disparo.puntoimpacto = hit.point;
                disparo.disparoarma = true;
                timeAux = Time.unscaledTime;

                if (vr) Debug.Log("VRRRR"); // Solo imprime si es VR
            }
        }
        if (!vr)
        {
            // Mover la cámara 
            PlayerLook();
        }
        // Verificar si el jugador está en el suelo
        CheckGroundStatus();

        // Aplicar movimiento y salto
        if (isWallRunning == false && isSliding == false && isOnRamp == false)
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
            StartCoroutine(Dash(Camera.main.transform.forward));
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
        if (isSliding && !groundedPlayer && !vr)
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
        if (vr)
        {
            Coliders();
        }

    }
    #endregion
    #region Jump and movement
    private void Jump()
    {

        // Saltar si está en el suelo
        if (controlador.Player.Jump.triggered && groundedPlayer && !isOnRamp && !tocandotecho || controlador.Player.Jump.triggered && isSliding && !isOnRamp && !tocandotecho)
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
        Vector3 origin = mesh.transform.position;
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
        if (!vr)
        { move = (transform.forward * move.z + transform.right * move.x).normalized; }
        else
        {
            move = (mesh.transform.forward * move.z + mesh.transform.right * move.x).normalized;
        }
        speedx = input.y;
        speedz = input.x;
        characterController.Move(move * Time.unscaledDeltaTime * playerSpeed);

        if (speedx < 0 && run == true && groundedPlayer == true
            || speedz < 0 && run == true && groundedPlayer == true
           || speedx > 0 && run == true && groundedPlayer == true
           || speedz > 0 && run == true && groundedPlayer == true)
            StartCoroutine(Runsound());

    }
    private void ApplyGravity()
    {
        if (!isWallRunning && !finish)
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
        // Guardar el color original de la luz
        originalColordash = DashLight.color;

        // Cambiar intensidad a 0 e iniciar el color rojo
        DashLight.intensity = 0;
        DashLight.color = Color.red; // Cambiar el color a rojo
        dashEnable = false;

        float startTime = Time.unscaledTime;
        DashParticles.gameObject.SetActive(true);

        // Movimiento del dash
        while (Time.unscaledTime < startTime + wallJumpTime)
        {
            characterController.Move(moveDir * dashSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        DashParticles.gameObject.SetActive(false);

        // Proceso de recarga
        float elapsedTime = 0f;
        float rechargeTime = dashCooldown; // Tiempo que tarda en recargarse completamente
        targetIntensitydash = 4f; // Intensidad máxima de la luz

        while (elapsedTime < rechargeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rechargeTime;

            // Interpolar intensidad de la luz y color (rojo -> original)
            DashLight.intensity = Mathf.Lerp(0, targetIntensitydash, t);
            DashLight.color = Color.Lerp(Color.red, originalColordash, t);
            yield return null;
        }

        // Asegurar que la luz está completamente cargada
        DashLight.intensity = targetIntensitydash;
        DashLight.color = originalColordash;
        dashEnable = true;
    }

    private void Coliders()
    {
        if (!isSliding)
        {
            Debug.Log("no deslizandose");
            capsuleCollider.center = mesh.transform.localPosition + new Vector3(0, .9f, 0);
            characterController.center = mesh.transform.localPosition + new Vector3(0, .9f, 0);
        }
        else
        {
            Debug.Log("deslizandose");
            capsuleCollider.center = mesh.transform.localPosition + new Vector3(0, .45f, 0);
            characterController.center = mesh.transform.localPosition + new Vector3(0, .45f, 0);
        }
    }

    #region Wallrun
    private void CheckForWall()
    {
        Vector3 positionray;
        int excludeWallLayer = ~wallLayer.value; // Invertir bitmask para excluir la capa específica
        if (!vr)
        {
            positionray = new Vector3(transform.position.x, transform.position.y + salidarayos, transform.position.z);
        }

        else
        {
            positionray = new Vector3(mesh.transform.position.x, mesh.transform.position.y + salidarayos, mesh.transform.position.z);
        }


        if (!vr)
        {
            // Detectar paredes a los lados del jugador con SphereCast
            Debug.DrawRay(positionray, -transform.right * wallDetectionDistance, Color.red);
            Debug.DrawRay(positionray, transform.right * wallDetectionDistance, Color.blue);
            wallLeft = Physics.SphereCast(positionray, sphereRadius, -transform.right, out hitLeft, wallDetectionDistance, excludeWallLayer);
            wallRight = Physics.SphereCast(positionray, sphereRadius, transform.right, out hitRight, wallDetectionDistance, excludeWallLayer);

        }
        else
        {
            Debug.DrawRay(positionray, mesh.transform.right * wallDetectionDistance, Color.red);
            Debug.DrawRay(positionray, -mesh.transform.right * wallDetectionDistance, Color.blue);
            wallLeft = Physics.SphereCast(positionray, sphereRadius, -mesh.transform.right, out hitLeft, wallDetectionDistance, excludeWallLayer);
            wallRight = Physics.SphereCast(positionray, sphereRadius, mesh.transform.right, out hitRight, wallDetectionDistance, excludeWallLayer);
        }



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
        if (!vr)
        {
            // Almacenar la dirección de entrada al wall run (la dirección de movimiento al momento de entrar)
            entryDirection = transform.forward;
        }
        else
        {
            // Almacenar la dirección de entrada al wall run (la dirección de movimiento al momento de entrar)
            entryDirection = mesh.transform.forward;
        }
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
        playerVelocity = wallRunDirection * wallRunSpeed;
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
            playerVelocity.y += gravityValue * Time.unscaledDeltaTime;
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
        // Guardar el color original de la luz
        originalColortime = TimeLight.color;

        // Cambiar intensidad a 0 e iniciar el color rojo
        TimeLight.intensity = 0;
        TimeLight.color = Color.red; // Cambiar el color a rojo
        Animationtime = 5;
        timeslow = false;

        // Activar el tiempo ralentizado
        SlowDownTime();
        yield return new WaitForSeconds(TimeSlowed * 0.2f);

        // Restaurar la animación
        Animationtime = 1;
        RestoreTime();

        // Proceso de recarga
        float elapsedTime = 0f;
        float rechargeTime = TimeCooldown; // Tiempo que tarda en recargarse completamente
        targetIntensitytime = 4f; // Intensidad máxima de la luz

        while (elapsedTime < rechargeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rechargeTime;

            // Interpolar intensidad de la luz y color (rojo -> original)
            TimeLight.intensity = Mathf.Lerp(0, targetIntensitytime, t);
            TimeLight.color = Color.Lerp(Color.red, originalColortime, t);
            yield return null;
        }

        // Asegurar que la luz está completamente cargada
        TimeLight.intensity = targetIntensitytime;
        TimeLight.color = originalColortime;
        timeslow = true;
    }

    // Ralentiza el tiempo al 20% de su velocidad normal
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
        if (!vr)
        {
            characterController.center = new Vector3(0f, crouchHeight / 2, 0f);
        }
        else
        {
            characterController.center = new Vector3(mesh.transform.localPosition.x, .45f, mesh.transform.localPosition.z);
        }


        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        // Verifica si hay un Capsule Collider
        if (capsuleCollider != null)
        {
            if (!vr)
            {
                // Modificar el centro del collider
                capsuleCollider.center = new Vector3(0f, crouchHeight / 2, 0f); // Cambia las coordenadas según necesites
            }
            else
            {
                capsuleCollider.center = characterController.center = new Vector3(mesh.transform.localPosition.x, .45f, mesh.transform.localPosition.z);
            }
            // Modificar la altura del collider
            capsuleCollider.height = crouchHeight; // Cambia este valor según necesites

            Debug.Log("Capsule Collider modificado.");
        }
        if (!vr)
        {
            // Capturar la dirección de movimiento actual
            slideDirection = transform.forward * slideSpeed;
        }
        else
        {
            slideDirection = mesh.transform.forward * slideSpeed;
        }
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
            slideTimer = 0.5f; // Extender temporalmente el deslizamiento
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
        if (!vr)
        {

            characterController.center = new Vector3(0f, originalHeight / 2, 0f);
        }
        else
        {
            characterController.center = characterController.center = new Vector3(mesh.transform.localPosition.x, .9f, mesh.transform.localPosition.z);
        }

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        // Verifica si hay un Capsule Collider
        if (capsuleCollider != null)
        {

            if (!vr)
            {

                // Modificar el centro del collider
                capsuleCollider.center = new Vector3(0f, originalHeight / 2, 0f); // Cambia las coordenadas según necesites
            }
            else
            {
                capsuleCollider.center = characterController.center = new Vector3(mesh.transform.localPosition.x, .9f, mesh.transform.localPosition.z);
            }
            // Modificar la altura del collider
            capsuleCollider.height = originalHeight; // Cambia este valor según necesites

            Debug.Log("Capsule Collider modificado.");
        }
        playerVelocity = new Vector3(0, playerVelocity.y, 0);
    }
    bool IsSomethingAbove()
    {
        // Comprobar si hay un objeto por encima
        //Vector3 top = transform.position + Vector3.up * (originalHeight / 2);
        //return Physics.CheckSphere(top, 0.1f, ceilingLayer);
        if (!vr)
        { return Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 2f, ceilingLayer); }
        else
        {
            return Physics.Raycast(mesh.transform.position, Vector3.up, out RaycastHit hit, 2f, ceilingLayer);
        }

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
        if (!vr)
        {
            // Verificar si el personaje está sobre una rampa
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, slideLayer))
            {
                return true;
            }
        }
        else
        {
            if (Physics.Raycast(mesh.transform.position, Vector3.down, out RaycastHit hit, 1f, slideLayer))
            {
                return true;
            }
        }

        return false;
    }

    Vector3 GetRampNormal()
    {
        if (!vr)
        {
            // Obtener la normal de la rampa debajo del personaje
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, slideLayer))
            {
                return hit.normal;
            }
        }
        else
        {
            if (Physics.Raycast(mesh.transform.position, Vector3.down, out RaycastHit hit, 1f, slideLayer))
            {
                return hit.normal;
            }
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

        if (other.CompareTag("checkpoint"))
        {
            checkpointposition = other.transform.position;
            checkpointrotation = other.transform.rotation;
            checkpointTime = currentTime;
            currentCheckpointID = other.GetComponent<Checkpoint>().checkpointID;
            other.gameObject.SetActive(false);
            Debug.Log("Checkpoint alcanzado: " + currentCheckpointID);
        }
       /* else if (other.CompareTag("Enemy"))
        {
            Muerto();
        }*/
        else if (other.CompareTag("finish"))
        {
            if (!vr)
            {
                finish = true;
                muñeco.gameObject.SetActive(true);
                CinematicCamera.gameObject.SetActive(true);
                characterController.Move(new Vector3(100000, 100000, 100000) * Time.unscaledDeltaTime);
                playerVelocity.y = 0;
                controlador.Disable();

            }

            StartCoroutine(finishlevel());
        }
        IEnumerator finishlevel()
        {
            if (!vr)
            {
                yield return new WaitForSeconds(3f);
            }

            UnifiedMenuController menuController = FindObjectOfType<UnifiedMenuController>();
            SaveSystem sistemaGuardado = FindObjectOfType<SaveSystem>();

            if (menuController != null)
            {
                sistemaGuardado.SaveNewTime(currentTime);
                menuController.ShowNextLevelMenu(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.LogError("No se encontró un objeto de tipo UnifiedMenuController en la escena.");
            }

            Time.timeScale = 0f;
        }
        // ✅ Se asigna como hijo del hijo de la plataforma
        if (other.CompareTag("Plataforma"))
        {
            currentPlatform = other.transform;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Plataforma"))
        {
            currentPlatform = null;
        }
    }


    /*
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
            botones.ApagarTodosLosBotones();
            TimeLight.intensity = targetIntensitytime;
            TimeLight.color = originalColortime;
            DashLight.intensity = targetIntensitydash;
            DashLight.color = originalColordash;
            timeslow = true;
            counter = false;
            Isparring = true;
            dashEnable = true;
            parry.gameObject.SetActive(false);
            timelow.gameObject.SetActive(false);
            DashParticles.gameObject.SetActive(false);
            progress.PointsToOrigin();
            Animationtime = 1;
            run = true;

            Time.timeScale = 0;

            SaveSystem sistemaGuardado = FindObjectOfType<SaveSystem>();
            // Llamar a la pantalla de muerte
            UnifiedMenuController menuController = FindObjectOfType<UnifiedMenuController>();
            if (menuController != null)
            {
                if (survivalmode)
                {
                    sistemaGuardado.SaveNewTime(currentTime);
                    survivalmode = false;
                }
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
       */
    #endregion
    public void cameraoffset()
    {
        if (isSliding)
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = alturacamaraslide;
        }
        else
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = alturacamara;
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
        LineRendererProgress.delay = timeparry / 13;
        progress.StartUnloading();
        yield return new WaitForSeconds(timeparry);
        counter = false;
        parry.gameObject.SetActive(false);
        LineRendererProgress.delay = parrycooldown / 13;
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
        animate.SetFloat("Tiempo", Animationtime);
    }

   
    private void OnEnable()
    {
        controlador.Enable();
        
        
    }

    private void OnDisable()
    {
        controlador.Disable();
    }
    private void LateUpdate()
    {
        if (vr)
        { // Mover la cámara 
            PlayerLook();
        }

    }
}
