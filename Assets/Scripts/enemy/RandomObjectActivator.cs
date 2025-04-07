using UnityEngine;
using System.Collections.Generic;

public class RandomObjectActivator : MonoBehaviour
{
    public List<GameObject> objectsToActivate; // Lista de objetos a activar
    [Range(1, 100)] public int activationPercentage = 50; // Porcentaje de activación

    void Start()
    {
        ActivateRandomObjects();
    }

    void ActivateRandomObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (Random.Range(0, 100) < activationPercentage)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }
}
