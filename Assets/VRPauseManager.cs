using UnityEngine;

public class VRPauseManager : UnifiedMenuController
{
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseGame();
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            PauseGame();
        }
    }  
}
