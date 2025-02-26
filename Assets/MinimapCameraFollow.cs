using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform player;
    public float height = 20f;

    void LateUpdate()
    {
        if (player == null) return;

        // Mantener la cámara sobre el jugador
        transform.position = new Vector3(player.position.x, height, player.position.z);

        // Asegurar que la cámara solo mira hacia abajo sin rotar lateralmente
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
