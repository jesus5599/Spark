using System.Collections;
using UnityEngine;
public class musicfinal : MonoBehaviour
{
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(music());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator music()
    {
        
        
        yield return new WaitForSeconds(2.9f);
        
        usica.instancia.CambiarCancion(3); // Reemplaza con el índice de la canción
       
        Destroy(gameObject);

    }
}
