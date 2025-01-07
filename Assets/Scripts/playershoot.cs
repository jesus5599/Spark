using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playershoot : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.unscaledDeltaTime);
        StartCoroutine(Destroy());

    }
    IEnumerator Destroy()
    {

        yield return new WaitForSeconds(15);
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {

        Destroy(gameObject, 0);

    }
    private void OnTriggerEnter(Collider collision)
    {
        Destroy(gameObject, 0);

    }
}
