using UnityEngine;

public class InitializeMoney : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("MoneyInitialized"))
        {
            PlayerPrefs.SetInt("CurrentMoney", 0);
            PlayerPrefs.SetInt("MoneyInitialized", 1);
            PlayerPrefs.Save();
        }
    }
}
