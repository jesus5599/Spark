using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class destroyparticle : MonoBehaviour
{
    public float tiempo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Destroy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Destroy()
    {       
        yield return new WaitForSeconds(tiempo);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
