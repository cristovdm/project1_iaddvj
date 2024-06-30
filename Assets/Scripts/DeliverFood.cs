using Inventory.Model;
using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeliverFood : MonoBehaviour
{
    public AudioClip moneySound;
    public BoxCollider2D interactionArea;
    public TextboxAnimator textboxAnimator;
    private AudioSource audioSource;
    private InventoryController inventory;
    private List<string> stageDeliveriesList;
    private string currentDelivery;
    public SpriteRenderer spriteRenderer;
    private string foodCloudPath = "Sprites/FoodClouds/";
    private Money moneyScript;

    // Diccionario de precios
    private Dictionary<string, int> dishPrices = new Dictionary<string, int>()
    {
        { "Tomato Soup", 50 },
        { "Corn Soup", 50 },
        { "CarrotCake", 500 },
        { "CazuelaMarina", 350 },
        { "HuevoDuro", 25 },
        { "FriedEgg", 50 },
        { "PescadoCaldero", 100 },
        { "FriedFish", 200 },
        { "FriedFishAndEgg", 250 },
        { "PescadoHorno", 150 },
        { "PopCorn", 100 },
        { "Salad", 150 },
        { "SandwichDePescado", 250 },
        { "SopaTomateCrotones", 400 },
        { "SopaZanahoria", 50 },
        { "CarrotTomatoSalad", 150 },
        { "Cut Carrot", 25 },
        { "FriedCarrotBreadSticks", 450 },
        { "CutCorn", 25 },
        { "cutTomato", 25 },
        { "SaladWithEgg", 250 },
        { "Tomatican", 600 },
        { "tortillaZanahoria", 150 }
    };

    List<string> ShuffleList(List<string> list)
    {
        System.Random rng = new System.Random();
        return list.OrderBy(a => rng.Next()).ToList();
    }

    private void RenderSprite(string spriteName)
    {
        Sprite sprite = Resources.Load<Sprite>(foodCloudPath + spriteName);
        Debug.Log(sprite);
        spriteRenderer.sprite = sprite;
    }

    private void ShowFoodCloud()
    {
        if (currentDelivery != null)
        {
            switch (currentDelivery)
            {
                case "Tomato Soup":
                    RenderSprite("TomatoSoupCloud");
                    return;
                case "Corn Soup":
                    RenderSprite("CornSoupCloud");
                    return;
                case "CarrotCake":
                    RenderSprite("CarrotCakeCloud");
                    return;
                case "CazuelaMarina":
                    RenderSprite("CazuelaMarinaCloud");
                    return;
                case "HuevoDuro":
                    RenderSprite("huevoduroCloud");
                    return;
                case "FriedEgg":
                    RenderSprite("HuevofritoCloud");
                    return;
                case "PescadoCaldero":
                    RenderSprite("PescadoCalderoCloud");
                    return;
                case "FriedFish":
                    RenderSprite("pescadofritoCloud");
                    return;
                case "FriedFishAndEgg":
                    RenderSprite("pescadofritoeggCloud");
                    return;
                case "PescadoHorno":
                    RenderSprite("pescadoHornoCloud");
                    return;
                case "PopCorn":
                    RenderSprite("popcornCloud");
                    return;
                case "Salad":
                    RenderSprite("saladCloud");
                    return;
                case "SandwichDePescado":
                    RenderSprite("sandwichPescadoCloud");
                    return;
                case "SopaTomateCrotones":
                    RenderSprite("sopaTomateCrotonesCloud");
                    return;
                case "SopaZanahoria":
                    RenderSprite("sopaZanahoriaCloud");
                    return;
                case "CarrotTomatoSalad":
                    RenderSprite("carrotTomatoSaladCloud");
                    return;
                case "Cut Carrot":
                    RenderSprite("cutCarrotCloud");
                    return;
                case "FriedCarrotBreadSticks":
                    RenderSprite("friedCarrotBreadSticks");
                    return;
                case "CutCorn":
                    RenderSprite("cutCorn");
                    return;
                case "cutTomato":
                    RenderSprite("cutTomato");
                    return;
                case "SaladWithEgg":
                    RenderSprite("saladWithEggCloud");
                    return;
                case "Tomatican":
                    RenderSprite("tomaticanCloud");
                    return;
                case "tortillaZanahoria":
                    RenderSprite("TortillaZanahoriaCloud");
                    return;
            }
        }
    }

    private void SelectInitialFood()
    {
        stageDeliveriesList = LevelManager.instance.GetDishesForCurrentLevel();
        if (stageDeliveriesList == null || !stageDeliveriesList.Any())
        {
            Debug.LogError("No dishes found for the current level!");
            return;
        }
        stageDeliveriesList = ShuffleList(stageDeliveriesList);
        currentDelivery = stageDeliveriesList.First();
        Debug.Log(currentDelivery.ToString());
        stageDeliveriesList.RemoveAt(0);
        ShowFoodCloud();
    }

    private void SelectNextDelivery()
    {
        currentDelivery = stageDeliveriesList.First();
        stageDeliveriesList.RemoveAt(0);
        ShowFoodCloud();
    }

    private void EndStage()
    {
        LevelManager.instance.NextLevel();
        SceneManager.LoadScene("Maze");
    }

    private void CheckNextDelivery()
    {
        if (stageDeliveriesList.Count > 0)
        {
            SelectNextDelivery();
        }
        else
        {
            EndStage();
        }
    }

    public bool CheckCollision()
    {
        if (interactionArea == null)
        {
            Debug.LogError("Interaction Area has not been assigned in the inspector!");
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll(interactionArea.bounds.center, interactionArea.bounds.size, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsFoodAvailable()
    {
        InventoryItem inventoryItem = inventory.GetInventoryFirstItem();

        if (!inventoryItem.IsEmpty)
        {
            Debug.Log(inventoryItem.item.Name);
            textboxAnimator.ShowTextbox("$" + dishPrices[inventoryItem.item.Name].ToString());
            return true;
        }
        else return false;
    }

    private bool IsCorrectDelivery()
    {
        InventoryItem inventoryItem = inventory.GetInventoryFirstItem();
        if (!inventoryItem.IsEmpty)
        {
            if (inventoryItem.item.Name == currentDelivery)
            {
                inventory.playerInventoryFilled = false;
                return true;
            }
            else return false;
        }
        else return false;
    }

    void Start()
    {
        inventory = FindObjectOfType<InventoryController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (interactionArea == null)
        {
            interactionArea = GetComponent<BoxCollider2D>();
        }

        moneyScript = FindObjectOfType<Money>();

        SelectInitialFood();
    }

    void Update()
    {
        if (CheckCollision() && Input.GetKeyDown(KeyCode.E) && IsFoodAvailable() && IsCorrectDelivery())
        {
            audioSource.PlayOneShot(moneySound);
            inventory.TakeOutFirstItem();
            moneyScript.AddMoney(dishPrices[currentDelivery]);
            CheckNextDelivery();
        }
    }
}
