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

        levelDishes.Add("1-1", new List<string> { "CarrotTomatoSalad", "CutCarrot", "cutCorn", "cutTomato","Salad" });

        levelDishes.Add("1-2", new List<string> { "Tomato Soup", "Corn Soup", "HuevoDuro", "PescadoCaldero", "SopaZanahoria"});

        levelDishes.Add("1-3", new List<string> { "FriedFish", "FriedFishAndEgg", "FriedEgg", "SopaTomateCrotones", "FriedCarrotBreadSticks" });

        levelDishes.Add("1-4", new List<string> { "PopCorn", "CarrotCake", "PescadoHorno", "tortillaZanahoria" });

        levelDishes.Add("1-5", new List<string> { "SopaTomateCrotones", "SandwichDePescado", "CazuelaMarina" });

        levelDishes.Add("1-6", new List<string> { "SopaTomateCrotones", "PopCorn", "CazuelaMarina", "tortillaZanahoria", "FriedCarrotBreadSticks","CarrotTomatoSalad" });

        levelDishes.Add("1-7", new List<string> { "Tomatican", "FriedFishAndEgg" });

        levelDishes.Add("1-8", new List<string> { "Tomatican", "CarrotCake", "FriedCarrotBreadSticks", "CazuelaMarina", "SopaTomateCrotones" });
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
            return new List<string>();
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
