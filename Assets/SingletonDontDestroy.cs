using UnityEngine;

public class SingletonDontDestroy : MonoBehaviour
{
    private static SingletonDontDestroy instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destruye los duplicados
        }
    }
}
