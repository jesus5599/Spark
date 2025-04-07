using UnityEngine;

public class usica : MonoBehaviour
{
    public static usica instancia; // Singleton para acceder desde otros scripts
    public AudioSource audioSource;
    public AudioClip[] canciones;
    private int indiceActual = 0;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CambiarCancion(int indice)
    {
        if (indice >= 0 && indice < canciones.Length)
        {
            audioSource.Stop();
            audioSource.clip = canciones[indice];
            audioSource.Play();
        }
    }
}

