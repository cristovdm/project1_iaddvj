using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    public static Money Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI moneyText;
    private int currentMoney = 0;
    private const string MoneyKey = "CurrentMoney";  // Clave para PlayerPrefs

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
    }

    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Intentando agregar una cantidad negativa de dinero.");
            return;
        }
        currentMoney += amount;
        SaveMoney();
        UpdateMoneyUI();
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
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, currentMoney);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        currentMoney = PlayerPrefs.GetInt(MoneyKey, 0);  // Carga el dinero guardado o 0 si no existe
    }
}
