using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class destroyparticle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShowImpact());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ShowImpact()
    {       
        yield return new WaitForSeconds(4);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
