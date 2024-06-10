using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("LevelInitialized"))
        {
            PlayerPrefs.SetString("CurrentLevel", "Level1");
            PlayerPrefs.SetInt("LevelInitialized", 1);
            PlayerPrefs.Save();
        }
    }
}