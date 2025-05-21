using UnityEngine;

public static class SaveData
{
    public static int GetLvl(string id) =>
        PlayerPrefs.GetInt($"Passive_{id}", 0);

    public static void SetLvl(string id, int lvl)
    {
        PlayerPrefs.SetInt($"Passive_{id}", lvl);
        PlayerPrefs.Save();
    }
}
