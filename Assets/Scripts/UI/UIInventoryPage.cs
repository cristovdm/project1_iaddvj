using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab; 
    [SerializeField] private RectTransform contentPanel; 

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    public Sprite image; 
    public int quantity;

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize - 1; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);

            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }


    private void HandleItemSelection(UIInventoryItem UIIinventoryItem)
    {
       Debug.Log("hey"); 
        
        /*
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1) return;

        OnDescriptionRequested?.Invoke(index);
        */
    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {
        /*
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1) return;

        // Assuming you have declared OnSwapItems event somewhere
        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        HandleItemSelection(inventoryItemUI);
        */
    }


    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        /*
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1) return;

        currentlyDraggedItemIndex = index;
        HandleItemSelection(inventoryItemUI);
        OnStartDragging?.Invoke(index);
        */
    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        /*
        ResetDraggedItem();
        */
    }

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            /*
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnItemActionRequested?.Invoke(index);
            */
        }



    public void Show()
    {
        gameObject.SetActive(true);

        listOfUIItems[0].SetData(image, quantity); 
        // ResetSelection();
    }

    public void Hide()
    {
        // actionPanel.Toggle(false);
        gameObject.SetActive(false);
        // ResetDraggedItem();
    }
}
