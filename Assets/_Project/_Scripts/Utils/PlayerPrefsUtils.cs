using UnityEngine;

public static class PlayerPrefsUtils
{
    
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        return PlayerPrefs.GetInt(key) == 1;
    }

}
