using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaQuieta : MonoBehaviour
{
    public float speedUp = 3f;
    public float speedDown = 3f;
    public Transform targetUp;
    public Transform targetDown;

    private bool movingUp = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("La plataforma necesita un Rigidbody. Agregando automáticamente.");
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true; // Para evitar colisiones físicas no deseadas
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Para suavizar el movimiento
    }

    void FixedUpdate()
    {
        float speed = movingUp ? speedUp : speedDown;
        Vector3 target = movingUp ? targetUp.position : targetDown.position;

        rb.MovePosition(Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movingUp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movingUp = false;
        }
    }
}
