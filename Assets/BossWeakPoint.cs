using UnityEngine;

public class BossWeakPoint : MonoBehaviour
{
    public BossController boss;
    public GameObject cableroto;
    public GameObject cableentero;
    private void Start()
    {
        cableentero.SetActive(true);
        cableroto.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Proyectil")) // Si el jugador ataca el punto débil
        {
            cableentero.SetActive(false);
            cableroto.SetActive(true);
            Destroy(gameObject); // Se destruye el punto de vida
           
            boss.WeakPointDestroyed(); // Llama al jefe para registrar el daño
        }
    }
}
