using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab; 
    [SerializeField] private RectTransform contentPanel; 
    [SerializeField] private MouseFollower mouseFollower; 


    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    public Sprite image, image2; 
    public int quantity;

    private int currentlyDraggedItemIndex = -1; 

    public void Awake(){
        Hide(); 
        mouseFollower.Toggle(false); 
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
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
       listOfUIItems[0].Select(); 
        
        /*
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1) return;

        OnDescriptionRequested?.Invoke(index);
        */
    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {

        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1) 
        {
            mouseFollower.Toggle(false); 
            currentlyDraggedItemIndex = -1; 
            return; 

            listOfUIItems[currentlyDraggedItemIndex].SetData(index == 0 ? image: image2, quantity); 
            listOfUIItems[index].SetData(currentlyDraggedItemIndex == 0 ? image: image2, quantity); 
            mouseFollower.Toggle(false); 
            currentlyDraggedItemIndex = -1; 
        }

    }


    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1) return;
        currentlyDraggedItemIndex = index;
        HandleItemSelection(inventoryItemUI);
       // OnStartDragging?.Invoke(index);

        mouseFollower.Toggle(true); 
        mouseFollower.SetData(index == 0 ? image: image2, quantity); 
    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        mouseFollower.Toggle(false); 
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
        listOfUIItems[1].SetData(image2, quantity); 
        // ResetSelection();
    }

    public void Hide()
    {
        // actionPanel.Toggle(false);
        gameObject.SetActive(false);
        // ResetDraggedItem();
    }
}
