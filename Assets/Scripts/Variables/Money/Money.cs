using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    public static Money Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI moneyText;
    private int currentMoney = 0;
    private const string MoneyKey = "CurrentMoney";  
    public TextMeshProUGUI moneyDisplay;

    private int debt = 100000;
    public TextMeshProUGUI debtText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMoney();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateMoneyUI();
        UpdateMoneyUI2();
    }

    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Intentando agregar una cantidad negativa de dinero.");
            return;
        }
        currentMoney += amount;
        debt -= amount;

        SaveMoney();
        UpdateMoneyUI();
        UpdateMoneyUI2();
        UpdateDebtUI();
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
        SaveMoney();
        UpdateMoneyUI();
        UpdateMoneyUI2();
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"{currentMoney}";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is not assigned.");
        }
    }

    public void SetMoneyTextReference(TextMeshProUGUI text)
    {
        moneyText = text;
        UpdateMoneyUI();
        UpdateMoneyUI2();
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, currentMoney);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        currentMoney = PlayerPrefs.GetInt(MoneyKey, 0);  
    }

    private void UpdateMoneyUI2()
    {
        if (moneyDisplay != null)
        {
            moneyDisplay.text = $"{currentMoney}";
            moneyDisplay.text = $"Money: ${currentMoney}"; 
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is not assigned.");
        }
    }

    private void UpdateDebtUI()
    {
        if (debtText != null)
        {
            debtText.text = $"Debt: {debt}";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component for debt is not assigned.");
        }
    }

    public void SetDebtTextReference(TextMeshProUGUI text)
    {
        debtText = text;
        UpdateDebtUI();
    }
}