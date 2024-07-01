using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int currentLevel = 1;
    public Dictionary<int, List<string>> levelDishes;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> levelMusic;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLevels();
            PlayMusicForCurrentLevel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeLevels()
    {
        levelDishes = new Dictionary<int, List<string>>();

        levelDishes.Add(1, new List<string> { "Cut Carrot", "Salad" });
        levelDishes.Add(2, new List<string> { "Tomato Soup", "Corn Soup", "HuevoDuro", "PescadoCaldero", "SopaZanahoria" });
        levelDishes.Add(3, new List<string> { "FriedFish", "FriedFishAndEgg", "FriedEgg", "SopaTomateCrotones", "FriedCarrotBreadSticks" });
        levelDishes.Add(4, new List<string> { "PopCorn", "CarrotCake", "PescadoHorno", "tortillaZanahoria" });
        levelDishes.Add(5, new List<string> { "SopaTomateCrotones", "SandwichDePescado", "CazuelaMarina" });
        levelDishes.Add(6, new List<string> { "SopaTomateCrotones", "PopCorn", "CazuelaMarina", "tortillaZanahoria", "FriedCarrotBreadSticks", "CarrotTomatoSalad" });
        levelDishes.Add(7, new List<string> { "Tomatican", "FriedFishAndEgg" });
        levelDishes.Add(8, new List<string> { "Tomatican", "CarrotCake", "FriedCarrotBreadSticks", "CazuelaMarina", "SopaTomateCrotones" });
    }

    void PlayMusicForCurrentLevel()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!");
            return;
        }

        if (levelMusic == null || levelMusic.Count == 0)
        {
            Debug.LogWarning("Level music list is not assigned or empty!");
            return;
        }

        // Selecciona una canción aleatoria de la lista
        int randomIndex = Random.Range(0, levelMusic.Count);
        AudioClip randomClip = levelMusic[randomIndex];

        Debug.Log($"Playing random music for level {currentLevel}: {randomClip.name}");
        audioSource.clip = randomClip;
        audioSource.Stop();
        audioSource.Play();
    }

    public float GetCountdownTimeForCurrentLevel()
    {
        float defaultTime = 120f;
        if (levelDishes.ContainsKey(currentLevel))
        {
            switch (currentLevel)
            {
                case 1:
                    return 180f;
                case 2:
                    return 180f;
                case 3:
                    return 240f;
                case 4:
                    return 240f;
                case 5:
                    return 240f;
                case 6:
                    return 300f;
                case 7:
                    return 360f;
                case 8:
                    return 360f;
                default:
                    return defaultTime;
            }
        }
        else
        {
            return defaultTime;
        }
    }

    public List<string> GetDishesForCurrentLevel()
    {
        if (levelDishes.ContainsKey(currentLevel))
        {
            return new List<string>(levelDishes[currentLevel]);
        }
        else
        {
            return new List<string>();
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        if (!levelDishes.ContainsKey(currentLevel))
        {
            currentLevel = 1;  //AGREGAR CONDICION DE TERMINO ACA,
        }
        PlayMusicForCurrentLevel();
    }
}
