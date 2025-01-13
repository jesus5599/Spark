using UnityEngine;

public class SwordCooldown : MonoBehaviour
{
    public ParticleSystem particleSystem; // Sistema de partículas
    public float cooldownTime = 5f;      // Tiempo de recarga
    private float currentCooldown;       // Tiempo restante
    public static bool isAvailable = true;     // Si la espada está disponible

    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.MainModule mainModule;

    void Start()
    {
        // Obtener los módulos del sistema de partículas
        emissionModule = particleSystem.emission;
        mainModule = particleSystem.main;

        // Inicialmente, habilitar las partículas
        emissionModule.enabled = true;
    }

    void Update()
    {
        if (!isAvailable)
        {
            // Reducir el cooldown
            currentCooldown -= Time.deltaTime;

            // Actualizar el brillo (por ejemplo, reducir el tamaño de las partículas)
            mainModule.startSize = Mathf.Lerp(0.1f, 1f, currentCooldown / cooldownTime);

            // Verificar si el cooldown terminó
            if (currentCooldown <= 0)
            {
                isAvailable = true;
                emissionModule.enabled = true; // Reactivar partículas
            }
        }
    }

    public void UseSword()
    {
        if (isAvailable)
        {
            // Acciones de la espada
            Debug.Log("Espada usada");

            // Iniciar el cooldown
            currentCooldown = cooldownTime;
            isAvailable = false;

            // Desactivar partículas
            emissionModule.enabled = false;
        }
    }
}
