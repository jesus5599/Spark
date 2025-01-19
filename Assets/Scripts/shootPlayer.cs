using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootPlayer : MonoBehaviour
{
    public float speed;
    private void Start()
    {
        transform.Rotate(90f,0f,0f);
        StartCoroutine(Destroy());
    }
    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.unscaledDeltaTime);

        if (UnifiedMenuController.isDeath)
        {
            gameObject.SetActive(false);
        }
    }
    IEnumerator Destroy()
    {
        
        yield return new WaitForSeconds(15);
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {


        collision.collider.GetComponent<Enemy>()?.Defeat();

        Destroy(gameObject);

    }
    
}