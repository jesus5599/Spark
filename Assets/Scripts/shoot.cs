using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        StartCoroutine(Destroy());

    }
    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);


    }
    IEnumerator Destroy()
    {

        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Destroy(gameObject);

    }
    private void OnTriggerEnter(Collider collision)
    {
collision.GetComponent<Collider>().GetComponent<parry>()?.Shoot();
        Destroy(gameObject);

    }
    private void OnTriggerStay(Collider other)
    {
        Destroy(gameObject);
    }
}