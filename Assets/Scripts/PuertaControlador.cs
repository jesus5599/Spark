using System.Collections.Generic;
using UnityEngine;

public class ControladorPuertasYBotones : MonoBehaviour
{
    public static ControladorPuertasYBotones Instance;

    [SerializeField] private Dictionary<int, List<Boton>> botonesPorID = new Dictionary<int, List<Boton>>();
    [SerializeField] private Dictionary<int, List<Puerta>> puertasPorID = new Dictionary<int, List<Puerta>>();
    [SerializeField] private Dictionary<int, int> botonesActivadosPorID = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RegistrarBoton(Boton boton)
    {
        if (!botonesPorID.ContainsKey(boton.puertaID))
        {
            botonesPorID[boton.puertaID] = new List<Boton>();
            botonesActivadosPorID[boton.puertaID] = 0;
        }

        botonesPorID[boton.puertaID].Add(boton);
    }

    public void RegistrarPuerta(Puerta puerta)
    {
        if (!puertasPorID.ContainsKey(puerta.puertaID))
        {
            puertasPorID[puerta.puertaID] = new List<Puerta>();
        }

        puertasPorID[puerta.puertaID].Add(puerta);
    }

    public void BotonActivado(int id)
    {
        if (botonesActivadosPorID.ContainsKey(id))
        {
            botonesActivadosPorID[id]++;
            VerificarEstadoPuertas(id);
        }
    }

    public void BotonDesactivado(int id)
    {
        if (botonesActivadosPorID.ContainsKey(id))
        {
            botonesActivadosPorID[id]--;
            VerificarEstadoPuertas(id);
        }
    }
    public void ApagarTodosLosBotones()
    {
        foreach (var kvp in botonesPorID)
        {
            foreach (var boton in kvp.Value)
            {
                boton.DesactivarBoton(); // Suponiendo que Boton tenga un método Desactivar()
            }

            botonesActivadosPorID[kvp.Key] = 0; // Restablecer el contador de botones activados
            VerificarEstadoPuertas(kvp.Key); // Actualizar el estado de las puertas
        }
    }

    private void VerificarEstadoPuertas(int id)
    {
        bool abrir = botonesActivadosPorID[id] == botonesPorID[id].Count;

        if (puertasPorID.ContainsKey(id))
        {
            foreach (Puerta puerta in puertasPorID[id])
            {
                puerta.CambiarEstado(abrir);
            }
        }
    }

    public bool TodosBotonesActivados(int id)
    {
        return botonesActivadosPorID.ContainsKey(id) && botonesActivadosPorID[id] == botonesPorID[id].Count;
    }
}
