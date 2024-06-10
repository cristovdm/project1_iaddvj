using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoinIngredients : MonoBehaviour
{
    [SerializeField] private InventorySO plateinventoryData;
    private bool isJoinable = false;
    [SerializeField] private Button joinButton;

    void Start()
    {
        CheckIfJoinable();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                OnButtonPress();
            }
        }
    }

    void CheckIfJoinable()
    {
        if (plateinventoryData.IsInventoryFull())
        {
            InventoryItem item_1 = plateinventoryData.GetItemAt(0);
            InventoryItem item_2 = plateinventoryData.GetItemAt(1);

            if ((item_1.item.name == "Bread" && item_2.item.name == "Carrot") ||
                (item_1.item.name == "Carrot" && item_2.item.name == "Bread"))
            {
                isJoinable = true;
            }
        }

        joinButton.interactable = isJoinable;
    }

    public void OnButtonPress()
    {
        if (isJoinable)
        {
            joinIngredients();
        }
    }

    void joinIngredients()
    {

        plateinventoryData.RemoveItem(0, 1);
        plateinventoryData.RemoveItem(1, 1);

        ItemSO joinedItem = ResourceManager.LoadResource<EdibleItemSO>("Tomato");
        InventoryItem item = new InventoryItem
        {
            item = joinedItem,
            quantity = 1,
            itemState = new System.Collections.Generic.List<ItemParameter>()
        };

        plateinventoryData.AddItem(item);

        joinButton.interactable = false;
    }
}
