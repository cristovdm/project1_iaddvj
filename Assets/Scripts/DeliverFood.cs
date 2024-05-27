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
                    Debug.Log("tomato sop");
                    RenderSprite("TomatoSoupCloud");
                    return;
                case "Corn Soup":
                    Debug.Log("corn sop");
                    RenderSprite("CornSoupCloud");
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
                    Debug.Log("FOOOOD");
                    textboxAnimator.ShowTextbox("$25");

                    return true;

                case "Corn Soup":
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

        SelectInitialFood();
    }

    void Update()
    {
        if (CheckCollision() && Input.GetKeyDown(KeyCode.E) && IsFoodAvailable() && IsCorrectDelivery())
        {
            audioSource.PlayOneShot(moneySound);
            inventory.TakeOutFirstItem();
            CheckNextDelivery();
        }
    }

}
