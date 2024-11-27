using UnityEngine;

public class Controladorjugador : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    [SerializeField]
    public bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    public Controlador controlador;
    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        controlador = new Controlador();
        groundedPlayer = true;
    }

    void Update()
    {
       
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            
        }
       
        Vector3 move = new Vector3(controlador.Player.Move.ReadValue<Vector2>().x, 0, controlador.Player.Move.ReadValue<Vector2>().y);
        controller.Move(move * Time.deltaTime * playerSpeed);
        
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Makes the player jump
        if (controlador.Player.Jump.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
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
