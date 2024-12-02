using UnityEngine;
using Cinemachine;
using static UnityEngine.UI.Image;
using System;
using UnityEngine.InputSystem.XR;
using System.Collections;

public class Controladorjugador : MonoBehaviour
{
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
    public LayerMask wallLayer;
    RaycastHit hitLeft, hitRight;
    

    // Variables para el control de la cámara con el ratón
    public CinemachineVirtualCamera virtualCamera; // Referencia a la Cinemachine Virtual Camera
    public Transform playerBody; // Referencia al cuerpo del jugador (para moverlo horizontalmente)
    public float SensitivityX = 2.0f; // Sensibilidad del ratón en el eje X
    public float SensitivityY = 2.0f; // Sensibilidad del ratón en el eje Y

    private float xRotation = 0f; // Rotación en el eje X (vertical)
    public Transform Cabeza;

    //Configuracion de dash
    public float dashSpeed;
    public bool dashEnable;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        controlador = new Controlador();
        virtualCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        dashEnable = true;
        
    }

    void Update()
    {
        // Mover la cámara con el ratón
        MouseLook();

        // Verificar si el jugador está en el suelo
        CheckGroundStatus();

        // Aplicar movimiento y salto
        HandleMovementAndJump();

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
        if (controlador.Player.Dash.triggered && dashEnable)
        {            
            StartCoroutine (Dash(( playerBody.position - virtualCamera.transform.position).normalized));
        }
        // Aplicar gravedad y mover el jugador
        ApplyGravity();
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void CheckGroundStatus()
    {
        // Usar raycast para verificar si el jugador está en el suelo
        Vector3 origin = transform.position;
        Vector3 direction = -transform.up;
        Debug.DrawRay(origin, direction * groundCheckDistance, Color.green);

        if (Physics.Raycast(origin, direction, groundCheckDistance, groundLayer))
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

    private void HandleMovementAndJump()
    {
        // Movimiento horizontal usando el sistema de entrada
        Vector2 input = controlador.Player.Move.ReadValue<Vector2>();
        ;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = virtualCamera.transform.TransformDirection(move);
        move.y = 0;
        move = (transform.forward * move.z + transform.right * move.x).normalized;
        characterController.Move(move * Time.deltaTime * playerSpeed);

        // Rotar al jugador en la dirección del movimiento
       if (move != Vector3.zero)
        {
            Vector3 moveDirectionNoRotation = Vector3.ProjectOnPlane(move, Vector3.up);  // Proyecta sobre el plano horizontal
            characterController.Move(moveDirectionNoRotation * Time.deltaTime * wallRunSpeed);
        }
        // Saltar si está en el suelo
        if (controlador.Player.Jump.triggered && groundedPlayer )
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
        if ( controlador.Player.Jump.triggered && isWallRunning)
        {
            WallJump();
        }
       
    }
    void WallJump()
    {
        isWallRunning = false;
        //wallRunTimer = wallRunDuration;
        StopWallRun();
        Vector3 forceToApply;
        if (wallLeft)
        {
        Vector3 wallNormalL = wallLeft ? hitLeft.normal : hitRight.normal;
             forceToApply = transform.up * wallJumpUpForce + wallNormalL * wallJumpSideForce;
            characterController.Move(forceToApply.normalized);
            playerVelocity.y += -9.81f * Time.deltaTime;
            StartCoroutine(Dash(forceToApply));
        }
        if (wallRight)
        {
        Vector3 wallNormalR = wallRight ? hitRight.normal : hitLeft.normal ;
             forceToApply = transform.up * wallJumpUpForce + wallNormalR * wallJumpSideForce;
            characterController.Move(forceToApply.normalized);
            playerVelocity.y += -9.81f * Time.deltaTime;
            //StartCoroutine(Dash(forceToApply));
        }     
    }

    IEnumerator Dash(Vector3 moveDir)
    {   dashEnable = false;
        float startTime = Time.time;

        while (Time.time < startTime + wallJumpTime)
        {
            characterController.Move(moveDir * dashSpeed * Time.fixedDeltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(4);
        dashEnable = true;
    }

    private void CheckForWall()
    {
        // Detectar paredes a los lados del jugador
        
        Debug.DrawRay(transform.position, -transform.right * wallDetectionDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * wallDetectionDistance, Color.blue);
         wallLeft = Physics.Raycast(transform.position, -transform.right, out hitLeft, wallDetectionDistance, wallLayer);
         wallRight = Physics.Raycast(transform.position, transform.right, out hitRight, wallDetectionDistance, wallLayer);

        if (wallLeft || wallRight)
        {
            wallNormal = wallLeft ? hitLeft.normal : hitRight.normal;
            StartWallRun();
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        wallRunTimer = wallRunDuration;

        // Guardar la dirección de movimiento previa
        preWallRunVelocity = playerVelocity;

        // Almacenar la dirección de entrada al wall run (la dirección de movimiento al momento de entrar)
        entryDirection = transform.forward; // Dirección a la que el jugador se mueve

        // Desactivar la gravedad temporalmente durante el wall run
        playerVelocity.y = 0; // Cancelar efecto de gravedad durante el wall run

        // Establecer la dirección del movimiento en base a la dirección de entrada
        playerVelocity = entryDirection * wallRunSpeed; // Movimiento horizontal sobre la pared
    }

    private void HandleWallRun()
    {
        wallRunTimer -= Time.deltaTime;

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


    }

    private void ApplyGravity()
    {
        if (!isWallRunning)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
    }

    private void MouseLook()
    {
        // Obtener el movimiento del ratón
        float mouseX = controlador.Player.Look.ReadValue<Vector2>().x * SensitivityX;
        float mouseY = controlador.Player.Look.ReadValue<Vector2>().y * SensitivityY;

        // Rotar la cámara vertical (eje X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar la rotación vertical

        // Aplicar rotación vertical a la cámara
        virtualCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Cabeza.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotar el cuerpo del jugador horizontalmente (eje Y)
        playerBody.Rotate(Vector3.up * mouseX);
        
        
    }

    private void OnEnable()
    {
        controlador.Enable();
    }

    private void OnDisable()
    {
        controlador.Disable();
    }
}
