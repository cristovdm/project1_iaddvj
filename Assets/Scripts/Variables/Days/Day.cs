using UnityEngine;
using TMPro;

public class Day : MonoBehaviour
{
    public static Day Instance { get; private set; }

    private int currentDay = 1;
    private const string DayKey = "CurrentDay";
    public TextMeshProUGUI dayDisplay;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDay();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateDayUI();
    }

    public void NextDay()
    {
        currentDay++;
        SaveDay();
        UpdateDayUI();
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    private void UpdateDayUI()
    {
        if (dayDisplay != null)
        {
            if (currentDay % 2 == 0)
            {
                dayDisplay.text = $"DAY: {currentDay}";
            }
            else
            {
                dayDisplay.text = $"NIGHT: {currentDay}";
            }
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is not assigned.");
        }
    }

    public void SetDayTextReference(TextMeshProUGUI text)
    {
        UpdateDayUI();
    }

    private void SaveDay()
    {
        PlayerPrefs.SetInt(DayKey, currentDay);
        PlayerPrefs.Save();
    }

    private void LoadDay()
    {
        currentDay = PlayerPrefs.GetInt(DayKey, 0);
    }
}