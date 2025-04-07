using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform player;
    public float height = 20f;

    void Start()
    {
        

        // Mantener la cámara sobre el jugador
        transform.position = new Vector3(transform.position.x, height, transform.position.z);

       
    }
}
