using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public static Level Instance { get; private set; }

    private string currentLevel;
    private const string LevelKey = "Level 1: Soup Time";
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

        Debug.Log("levelKey");
        Debug.Log(levelKey);

        switch (levelKey)
        {
            // Level 1
            case "1-2":
                currentLevel = "Level 1: Soup Time";
                break;
            // Level 2
            case "2-2":
                currentLevel = "Level 2: Gourmet Prep";
                break;
            // Level 3
            case "3-2":
                currentLevel = "Level 3: Heat Wave";
                break;
            // Level 4
            case "4-2":
                currentLevel = "Level 4: Bubble Urchin";
                break;
            // Level 5
            case "5-2":
                currentLevel = "Level 5: Banana Chaos";
                break;
            // Level 6
            case "6-2":
                currentLevel = "Level 6: Stalthy Stealers";
                break;
            // Level 7
            case "7-2":
                currentLevel = "Level 7: Final Feast";
                break;
            // Game Over
            default:
                currentLevel = "Game Over";
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
            levelDisplay.text = $"{currentLevel}";
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
        currentLevel = PlayerPrefs.GetString(LevelKey, "Level 1: Soup Time");
    }

    public void ResetLevel()
    {
        currentLevel = "Level 1: Soup Time";
        SaveLevel();
        UpdateLevelUI();
    }
}