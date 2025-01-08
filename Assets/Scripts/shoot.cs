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
        
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {


        if (collision.transform.CompareTag("Parry"))
        {
            transform.Rotate(180f, 0f, 0f);
        }
        if (collision.transform.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>()?.Defeat();
        }
        

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Parry"))
        {
            
            transform.Rotate(180f, 0f, 0f);
        }
        

    }
    private void OnTriggerStay(Collider other)
    {
        Destroy(gameObject);
    }
}