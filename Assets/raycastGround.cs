using UnityEngine;

public class raycastGround : MonoBehaviour
{ //public raycastGround rayo = Camera.main.ScreenPointToRay();
    public float longray;
    public Vector3 Origen, direccion;
    public LayerMask Ground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Origen = transform.position;
        direccion = -transform.up;
        Debug.DrawRay(Origen, direccion * longray, Color.green);
        Ray rayo = new Ray(Origen, direccion * longray) ;
       if (Physics.Raycast(rayo, longray, Ground))
        {
            GetComponentInParent<Controladorjugador>().groundedPlayer = true;
        }
        else { GetComponentInParent<Controladorjugador>().groundedPlayer = false; }
    }
    

}
