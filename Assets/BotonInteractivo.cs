using UnityEngine;

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

    private bool temporizadorActivaldo = false; // Para evitar el temporizador si todos están activados
    private bool todosActivados = false; // Si todos los botones del grupo están activados

    private void Start()
    {
        render = GetComponent<Renderer>();
        ActualizarMaterial();
        ControladorPuertasYBotones.Instance.RegistrarBoton(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return; // Evitar múltiples activaciones

        ActivarBoton();
    }

    private void ActivarBoton()
    {
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

    private void DesactivarBoton()
    {
        if (!activado) return;

        activado = false;
        ControladorPuertasYBotones.Instance.BotonDesactivado(puertaID);
        ActualizarMaterial();
    }

    private void ActualizarMaterial()
    {
        render.material = activado ? materialActivado : materialDesactivado;
    }
}
