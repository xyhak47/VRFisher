using UnityEngine;
using System.Collections;
using Xxtea;

public static class PlayerPrefabWithXXTEA
{
    public static void SetInt(string key, int defaultValue = 0)
    {
        string data = XXTEA.EncryptToBase64String(defaultValue.ToString(), key);
        PlayerPrefs.SetString(key, data);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        string data = PlayerPrefs.GetString(key);
        string result = XXTEA.DecryptBase64StringToString(data, key);

        if (result == "") return defaultValue;

        return int.Parse(result);
    }
}
