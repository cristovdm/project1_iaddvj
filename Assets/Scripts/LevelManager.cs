using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int currentStage = 1;
    public int currentLevel = 1;
    public Dictionary<string, List<string>> levelDishes;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeLevels()
    {
        levelDishes = new Dictionary<string, List<string>>();

        // Nivel 1-1
        levelDishes.Add("1-1", new List<string> { "Tomato Soup", "Corn Soup" });

        // Nivel 1-2
        levelDishes.Add("1-2", new List<string> { "CarrotCake", "CazuelaMarina" });
    }

    public List<string> GetDishesForCurrentLevel()
    {
        string levelKey = currentStage + "-" + currentLevel;
        if (levelDishes.ContainsKey(levelKey))
        {
            return new List<string>(levelDishes[levelKey]);
        }
        else
        {
            return new List<string>(); // Nivel no definido
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        if (!levelDishes.ContainsKey(currentStage + "-" + currentLevel))
        {
            currentStage++;
            currentLevel = 1;
        }
    }
}
