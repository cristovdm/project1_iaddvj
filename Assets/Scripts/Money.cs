using UnityEngine;

public class Money : MonoBehaviour
{
    public static Money Instance { get; private set; }

    private int currentMoney = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Current player money");
            Debug.Log(currentMoney);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Intentando agregar una cantidad negativa de dinero.");
            return;
        }
        currentMoney += amount;
    }

    public void SubtractMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Intentando restar una cantidad negativa de dinero.");
            return;
        }
        if (currentMoney - amount < 0)
        {
            Debug.LogWarning("No se puede tener dinero negativo.");
            return;
        }
        currentMoney -= amount;
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}
