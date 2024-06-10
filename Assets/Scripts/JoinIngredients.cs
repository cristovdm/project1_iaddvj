using Inventory;
using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class JoinIngredients : MonoBehaviour
{
    [SerializeField]
    private InventorySO plateinventoryData;
    private bool isJoinable = false;

    private InventoryController plateinventory;
    private ItemSO joinedItem;

    void Start()
    {
        checkIfJoinable();
    }

    void checkIfJoinable() {
        if (plateinventoryData.IsInventoryFull())
        {
            InventoryItem item_1 = plateinventoryData.GetItemAt(0);
            InventoryItem item_2 = plateinventoryData.GetItemAt(1);

            if (item_1.item.name == "Bread" && item_2.item.name == "Carrot")
            {
               isJoinable = true;
            }

            if (item_1.item.name == "Carrot" && item_2.item.name == "Bread")
            {
                isJoinable = true;
            }

            if (isJoinable)
            {
                joinIngredients();
            }
        }
    }
    void joinIngredients()
    {
        plateinventoryData.RemoveItem(0, 1);
        plateinventoryData.RemoveItem(1, 1);
        joinedItem = ResourceManager.LoadResource<EdibleItemSO>("Tomato");
        InventoryItem item = new InventoryItem
        {
            item = joinedItem,
            quantity = 1,
            itemState = new List<ItemParameter>()
        };
        plateinventoryData.AddItem(item);
        isJoinable = false;
    }
}
