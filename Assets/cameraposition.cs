using Cinemachine;
using UnityEngine;

public class cameraposition : MonoBehaviour
{
    CinemachineVirtualCamera Vcamara;
    private void Awake()
    {
        Vcamara.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(-Camera.main.transform.position.x, Camera.main.transform.position.y, -Camera.main.transform.position.z);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("la posicion de la camara es" + Camera.main.transform.localPosition + "");
       
    }
}
