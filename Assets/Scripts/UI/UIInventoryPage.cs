using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.UI {
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private UIInventoryItem itemPrefab; 
        [SerializeField] private RectTransform contentPanel; 
        [SerializeField] private MouseFollower mouseFollower; 


        List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

        private int currentlyDraggedItemIndex = -1; 

        public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging; 

        public event Action<int, int> OnSwapItems; 

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

        internal void ResetAllItems(){
            foreach (var item in listOfUIItems)
            {
                item.ResetData(); 
                item.Deselect(); 
            }
        }

        public void UpdateData(int itemIndex,
            Sprite itemImage, int itemQuantity)
        {
            if (listOfUIItems.Count > itemIndex)
            {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name)
        {

            DeselectAllItems();
            listOfUIItems[itemIndex].Select();
        }


        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {

            {
                int index = listOfUIItems.IndexOf(inventoryItemUI);
                if (index == -1)
                    return;
            }
        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {

            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1) 
            {
                return; 
            }
                OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
                HandleItemSelection(inventoryItemUI);
            
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }


        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1) return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);

        }
        
        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();

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

            ResetSelection();

            // ResetSelection();
        }

        public void ResetSelection()
        {
            DeselectAllItems();
        }

        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
            // actionPanel.Toggle(false);
        }


        public void Hide()
        {
            // actionPanel.Toggle(false);
            gameObject.SetActive(false);
            ResetDraggedItem();
        }
    }
}
