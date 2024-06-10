using UnityEngine;
using TMPro;

public class Level : MonoBehaviour
{
    public static Level Instance { get; private set; }

    private string currentLevel;
    private const string LevelKey = "CurrentLevel";
    public TextMeshProUGUI levelDisplay;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLevel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateLevelUI();
    }

    public void SetLevel(int levelNumber1, int dayOrNight)
    {

        // day = 1, night = 2
        string levelKey = $"{levelNumber1}-{dayOrNight}";

        switch (levelKey)
        {
            case "1-1":
                currentLevel = "Level1-1";
                break;
            case "1-2":
                currentLevel = "Level1-2";
                break;
            case "2-1":
                currentLevel = "Level2-1";
                break;
            case "2-2":
                currentLevel = "Level2-2";
                break;
            // Add more cases as needed
            default:
                Debug.LogWarning($"Unknown level key: {levelKey}");
                return;
        }

        SaveLevel();
        UpdateLevelUI();
    }

    public string GetCurrentLevel()
    {
        return currentLevel;
    }

    private void UpdateLevelUI()
    {
        if (levelDisplay != null)
        {
            levelDisplay.text = $"Level: {currentLevel}";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is not assigned.");
        }
    }

    public void SetLevelTextReference(TextMeshProUGUI text)
    {
        levelDisplay = text;
        UpdateLevelUI();
    }

    private void SaveLevel()
    {
        PlayerPrefs.SetString(LevelKey, currentLevel);
        PlayerPrefs.Save();
    }

    private void LoadLevel()
    {
        currentLevel = PlayerPrefs.GetString(LevelKey, "Level 1-1");
    }
}