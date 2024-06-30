using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class JoinIngredients : MonoBehaviour
{
    [SerializeField] private InventorySO plateinventoryData;
    private bool isJoinable = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip joinSound;
    [SerializeField] private TMP_Text joinButtonText;
    [SerializeField] private Image isMixable;


    void Start()
    {
        CheckIfJoinable(); 
    }

    void Update()
    {
        CheckIfJoinable(); 
        isMixable.enabled = isJoinable; 
    }

    void CheckIfJoinable()
    {
        if (plateinventoryData.IsInventoryFull())
        {
            InventoryItem item_1 = plateinventoryData.GetItemAt(0);
            InventoryItem item_2 = plateinventoryData.GetItemAt(1);

            string combinedItemName = GetCombinedItemName(item_1.item.name, item_2.item.name);
            isJoinable = !string.IsNullOrEmpty(combinedItemName);
        }
        else
        {
            isJoinable = false;
        }

        if (isJoinable){
            joinButtonText.text = "Mix!"; 
        }
        else{
            joinButtonText.text = ""; 
        }
    }

    public void Join()
    {
        if (plateinventoryData.IsInventoryFull())
        {
            InventoryItem item_1 = plateinventoryData.GetItemAt(0);
            InventoryItem item_2 = plateinventoryData.GetItemAt(1);
    
            string combinedItemName = GetCombinedItemName(item_1.item.name, item_2.item.name);

            if (!string.IsNullOrEmpty(combinedItemName))
            {
                audioSource.PlayOneShot(joinSound);
                plateinventoryData.RemoveItem(0, 1);
                plateinventoryData.RemoveItem(1, 1);

                ItemSO joinedItem = ResourceManager.LoadResource<EdibleItemSO>(combinedItemName);
                InventoryItem newItem = new InventoryItem
                {
                    item = joinedItem,
                    quantity = 1,
                    itemState = new List<ItemParameter>()
                };
             

                plateinventoryData.AddItem(newItem);
            }

        }
    }

    string GetCombinedItemName(string item1, string item2)
    {
        var combinations = new Dictionary<(string, string), string>
        {
            { ("FriedFish", "FriedEgg"), "FriedFishAndEgg" },
            { ("FriedEgg", "FriedFish"), "FriedFishAndEgg" },
            { ("Carrot", "Egg"), "tortillaZanahoria" },
            { ("Egg", "Carrot"), "tortillaZanahoria" },
            { ("Cut Carrot", "Egg"), "tortillaZanahoria" },
            { ("Egg", "Cut Carrot"), "tortillaZanahoria" },
            { ("Cut Carrot", "CutTomato"), "CarrotTomatoSalad" },
            { ("CutTomato", "Cut Carrot"), "CarrotTomatoSalad" },
            { ("SopaZanahoria", "Egg"), "CarrotSoupEgg" },
            { ("Egg", "SopaZanahoria"), "CarrotSoupEgg" },
            { ("CutTomato", "CutCorn"), "Salad" },
            { ("CutCorn", "CutTomato"), "Salad" },
            { ("Salad", "Egg"), "SaladWithEgg" },
            { ("Egg", "Salad"), "SaladWithEgg" },
            { ("SaladWithEgg", "Cut Carrot"), "Tomatican" },
            { ("Cut Carrot", "SaladWithEgg"), "Tomatican" },
            { ("FriedFish", "PanCortado"), "SandwichDePescado" },
            { ("PanCortado", "FriedFish"), "SandwichDePescado" },
            { ("TomatoSoup", "PanCortadoFrito"), "SopaTomateCrotones" },
            { ("PanCortadoFrito", "TomatoSoup"), "SopaTomateCrotones" },
            { ("PescadoCaldero", "CornSoup"), "CazuelaMarina" },
            { ("CornSoup", "PescadoCaldero"), "CazuelaMarina" },
            { ("Cut Carrot", "Bread"), "BreadCarrotSticks" },
            { ("Bread", "Cut Carrot"), "BreadCarrotSticks" },
        };

        if (combinations.TryGetValue((item1, item2), out string result) ||
            combinations.TryGetValue((item2, item1), out result))
        {
            return result;
        }
        return null;
    }
}
