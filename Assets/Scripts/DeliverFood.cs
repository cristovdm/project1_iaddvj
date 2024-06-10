using Inventory;
using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeliverFood : MonoBehaviour
{
    public AudioClip moneySound;
    public BoxCollider2D interactionArea;
    public TextboxAnimator textboxAnimator;
    private AudioSource audioSource;
    private InventoryController inventory;
    public List<string> stageDeliveriesList;
    private string currentDelivery;
    public SpriteRenderer spriteRenderer;
    private string foodCloudPath = "Sprites/FoodClouds/";

    private Money moneyScript;  // Referencia al script Money

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
            }
        }
    }

    private void SelectInitialFood()
    {
        if (stageDeliveriesList == null || !stageDeliveriesList.Any())
        {
            //Vacio
        }
        stageDeliveriesList = ShuffleList(stageDeliveriesList);
        currentDelivery = stageDeliveriesList.First();
        stageDeliveriesList.RemoveAt(0);
        //stageDeliveriesList.RemoveAt(0); //BORRAR PARA QUE FUNCIONE NORMALMENTE
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
        SceneManager.LoadScene("Maze");
    }

    private void CheckNextDelivery()
    {
        Debug.Log(stageDeliveriesList.Count);
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
            switch (inventoryItem.item.Name)
            {
                case "Tomato Soup":
                    textboxAnimator.ShowTextbox("$25");

                    return true;
                case "Corn Soup":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "CarrotCake":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "CazuelaMarina":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "HuevoDuro":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "FriedEgg":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "PescadoCaldero":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "FriedFish":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "FriedFishAndEgg":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "PescadoHorno":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "PopCorn":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "Salad":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "SandwichDePescado":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "SopaTomateCrotones":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
                case "SopaZanahoria":
                    textboxAnimator.ShowTextbox("$25");
                    return true;
            }
            return false;
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
            moneyScript.AddMoney(25);
            CheckNextDelivery();
        }
    }
}
