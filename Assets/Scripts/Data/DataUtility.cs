using Unity.VisualScripting;
using UnityEngine;

    public static class DataUtility
    {

    public static void Initialize()
    {
    }
    public static int Level
    {
        get => PlayerPrefs.GetInt("LevelKey", 1);
        set
        {
            PlayerPrefs.SetInt("LevelKey", value);
        }
    }

}

