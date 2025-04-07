using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class activeBoss : MonoBehaviour
{
    public GameObject Boss,techo1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            StartCoroutine(StartBoss());
        }
    }
    IEnumerator StartBoss()
    {
        Boss.SetActive(true);
        yield return new WaitForSeconds(2f);
        techo1.SetActive(true);       
        Destroy(gameObject);

    }
}
