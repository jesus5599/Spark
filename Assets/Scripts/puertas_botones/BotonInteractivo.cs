using UnityEngine;
using UnityEngine.Audio;

public class Boton : MonoBehaviour
{
    public int puertaID;   // ID del grupo de botones
    private bool activado = false;
    private Renderer render;
    public Material materialActivado;
    public Material materialDesactivado;

    [Header("Opciones")]
    public bool seDesactivaPorTiempo = false;   // Si se desactiva por tiempo
    public float tiempoParaDesactivar = 5f;     // Tiempo para desactivar
    public bool mantenerActivadosCuandoTodos = false;   // Si se mantienen activados cuando todos estén activados
    public bool pisable, disparable;
    private bool temporizadorActivaldo = false; // Para evitar el temporizador si todos están activados
    private bool todosActivados = false; // Si todos los botones del grupo están activados
    public AudioClip sonidoActivo,sonidoDesactivo;

    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        render = GetComponent<Renderer>();
        ActualizarMaterial();
        ControladorPuertasYBotones.Instance.RegistrarBoton(this);
        if (pisable == false && disparable == false)
        {
            pisable = true;
            disparable = true;
        }
       
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (activado) return; // Evitar múltiples activaciones
        if (other.CompareTag("Player")) if(pisable == true && !activado)ActivarBoton();        
        if (other.CompareTag("Proyectil"))if (disparable == true && !activado) ActivarBoton();
    }

    private void ActivarBoton()
    {
        audioSource.PlayOneShot(sonidoActivo);
        activado = true;
        ControladorPuertasYBotones.Instance.BotonActivado(puertaID);
        ActualizarMaterial();

        // Verificar si todos los botones del grupo están activados
        todosActivados = ControladorPuertasYBotones.Instance.TodosBotonesActivados(puertaID);

        // Si todos los botones están activados
        if (todosActivados)
        {
            if (mantenerActivadosCuandoTodos)
            {
                // Si la opción está activada, no desactivar los botones
                temporizadorActivaldo = false;
            }
            else
            {
                // Si la opción no está activada, mantener temporizador activo
                temporizadorActivaldo = true;
                Invoke(nameof(DesactivarBoton), tiempoParaDesactivar);
            }
        }
        else if (seDesactivaPorTiempo && !mantenerActivadosCuandoTodos)
        {
            // Si no todos los botones están activados y el temporizador está activado
            temporizadorActivaldo = true;
            Invoke(nameof(DesactivarBoton), tiempoParaDesactivar);
        }
    }

    public void DesactivarBoton()
    {
       
        if (!activado) return;
        audioSource.PlayOneShot(sonidoDesactivo);
        activado = false;
        ControladorPuertasYBotones.Instance.BotonDesactivado(puertaID);
        ActualizarMaterial();
    }

    private void ActualizarMaterial()
    {
        render.material = activado ? materialActivado : materialDesactivado;
    }
}
