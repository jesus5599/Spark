using UnityEngine;

public static class SensitivityManager
{
    private static string sensitivityKey = "SensitivityOffset";

    public static float GetSensitivityOffset()
    {
        return PlayerPrefs.GetFloat(sensitivityKey, 0f);
    }

    public static void SetSensitivityOffset(float offset)
    {
        PlayerPrefs.SetFloat(sensitivityKey, offset);
        PlayerPrefs.Save();
    }

    public static float ApplyToBase(float baseValue)
    {
        return baseValue + GetSensitivityOffset();
    }
}
