using UnityEngine;

public class LockCameraPosition : MonoBehaviour
{
    public float fixedX = 0f; // Valor fijo en X
    public float fixedZ = 0f; // Valor fijo en Z

    void LateUpdate()
    {
        transform.position = new Vector3(fixedX, transform.position.y, fixedZ);
    }
}
