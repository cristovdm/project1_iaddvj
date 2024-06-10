using UnityEngine;

public class InitializeDay : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("DayInitialized"))
        {
            PlayerPrefs.SetInt("CurrentDay", 1);
            PlayerPrefs.SetInt("DayInitialized", 1);
            PlayerPrefs.Save();
        }
    }
}