using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryPage inventoryUI;

        [SerializeField]
        private UIInventoryPage trashinventoryUI;

        [SerializeField]
        private UIInventoryPage plateinventoryUI;

        [SerializeField]
        private InventorySO inventoryData;

        [SerializeField]
        private InventorySO trashinventoryData;

        [SerializeField]
        private InventorySO plateinventoryData;

        private bool playerInventoryEmpty = false; 

        public List<InventoryItem> initialItems = new List<InventoryItem>();
        public List<InventoryItem> TrashInitialItems = new List<InventoryItem>();
        public List<InventoryItem> PlateInitialItems = new List<InventoryItem>();

        [SerializeField]
        private AudioClip dropClip;

        [SerializeField]
        private AudioSource audioSource;

        private string name;

        private int lastDraggedIndex = 0; 

        private void Start()
        {
            PrepareUI();
            PrepareInventoryData();

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Kitchen")
            {
                PreparePlateInventoryData();
                foreach (var item in trashinventoryData.GetCurrentInventoryState())
                {
                    TrashInitialItems.Add(new InventoryItem { item = item.Value.item, quantity = item.Value.quantity });
                    
                }
                inventoryUI.Show();
            }

            PrepareTrashInventoryData(); 


        }

        private void SaveInventoryData()
        {
            TrashInitialItems.Clear();
            foreach (var item in inventoryData.GetCurrentInventoryState())
            {
                InventoryItem add_variable = new InventoryItem { item = item.Value.item, quantity = item.Value.quantity };
                trashinventoryData.AddItem(add_variable);
                
            }

        }


        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void PrepareTrashInventoryData()
        {
            trashinventoryData.Initialize();
            trashinventoryData.OnInventoryUpdated += UpdateTrashInventoryUI;
            foreach (InventoryItem item in TrashInitialItems)
            {
                if (item.IsEmpty)
                    continue;
                
                trashinventoryData.AddItem(item);
            }
        }
        
        private void PreparePlateInventoryData()
        {
            plateinventoryData.Initialize();
            plateinventoryData.OnInventoryUpdated += UpdatePlateInventoryUI;
            foreach (InventoryItem item in PlateInitialItems)
            {
                if (item.IsEmpty)
                    continue;

                plateinventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
            }
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen"){
                SaveInventoryData(); 
            }
        }

        private void UpdateTrashInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            trashinventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                trashinventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
            }
        }
        
        private void UpdatePlateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            plateinventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                plateinventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Kitchen")
            {
                trashinventoryUI.InitializeInventoryUI(trashinventoryData.Size);
                trashinventoryUI.OnDescriptionRequested += HandleTrashDescriptionRequest;
                trashinventoryUI.OnSwapItems += HandleTrashSwapItems;
                trashinventoryUI.OnStartDragging += HandleTrashDragging;
                trashinventoryUI.OnItemActionRequested += HandleTrashItemActionRequest;

                plateinventoryUI.InitializeInventoryUI(plateinventoryData.Size);
                plateinventoryUI.OnDescriptionRequested += HandlePlateDescriptionRequest;
                plateinventoryUI.OnSwapItems += HandlePlateSwapItems;
                plateinventoryUI.OnStartDragging += HandlePlateDragging;
                plateinventoryUI.OnItemActionRequested += HandlePlateItemActionRequest;
            }
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }
        }

        private void HandleTrashItemActionRequest(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = trashinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                trashinventoryUI.ShowItemAction(itemIndex);
                trashinventoryUI.AddAction(itemAction.ActionName, () => PerformTrashAction(itemIndex));
            }
        }

        private void HandlePlateItemActionRequest(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = plateinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                plateinventoryUI.ShowItemAction(itemIndex);
                plateinventoryUI.AddAction(itemAction.ActionName, () => PerformPlateAction(itemIndex));
            }
        }

        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        private void DropTrashItem(int itemIndex, int quantity)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;
            
            trashinventoryData.RemoveItem(itemIndex, quantity);
            trashinventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        private void DropPlateItem(int itemIndex, int quantity)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            plateinventoryData.RemoveItem(itemIndex, quantity);
            plateinventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        public void PerformAction(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    inventoryUI.ResetSelection();
            }
        }

        public void PerformTrashAction(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = trashinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                trashinventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (trashinventoryData.GetItemAt(itemIndex).IsEmpty)
                    trashinventoryUI.ResetSelection();
            }
        }

        public void PerformPlateAction(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = plateinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                plateinventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (plateinventoryData.GetItemAt(itemIndex).IsEmpty)
                    plateinventoryUI.ResetSelection();
            }
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity, itemIndex, inventoryData.name);
        }

        private void HandleTrashDragging(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = trashinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            trashinventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity, itemIndex, trashinventoryData.name);
        }

        private void HandlePlateDragging(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = plateinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            plateinventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity, itemIndex, plateinventoryData.name);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            if (inventoryData.SwapItems(itemIndex_1, itemIndex_2))
            {
            }
            else if (inventoryData.name == "PlayerInventory")
            {
                if (name == "TrashInventory")
                {
                    if (playerInventoryEmpty)
                    {
                        InventoryItem trashItem = inventoryData.GetItemAt(0);
                        trashinventoryData.AddItem(trashItem);
                        DropItem(0, 1); 
                    }
                    
                    InventoryItem newItem = trashinventoryData.GetItemAt(lastDraggedIndex);
                    newItem.quantity = 1;
                    AddInventoryItem(newItem);
                    DropTrashItem(lastDraggedIndex, 1);
                    playerInventoryEmpty = true;
                }
                else if (name == "PlateInventory")
                {
                    if (playerInventoryEmpty)
                    {
                        InventoryItem plateItem = inventoryData.GetItemAt(0);
                        plateinventoryData.AddItem(plateItem);
                        DropItem(0, 1);
                    }

                    InventoryItem newItem = plateinventoryData.GetItemAt(lastDraggedIndex);
                    newItem.quantity = 1;
                    AddInventoryItem(newItem);
                    DropPlateItem(lastDraggedIndex, 1);
                    playerInventoryEmpty = true;
                }
            }
        }

        private void HandleTrashSwapItems(int itemIndex_1, int itemIndex_2)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            if (trashinventoryData.SwapItems(itemIndex_1, itemIndex_2)){
            }

            else {
                if (trashinventoryData.IsInventoryFull()){
                    //HandleSwapItems(itemIndex_1, itemIndex_2); 
                    //playerInventoryEmpty = false; 
                }
                else{
                 InventoryItem trashItem = inventoryData.GetItemAt(0);
                 trashinventoryData.AddItem(trashItem);
                 DropItem(0, 1);
                 playerInventoryEmpty = false; 
                }
            }
        }

        private void HandlePlateSwapItems(int itemIndex_1, int itemIndex_2)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            if (plateinventoryData.SwapItems(itemIndex_1, itemIndex_2)){
            }

            else {
                 if (plateinventoryData.IsInventoryFull()){
                    Debug.Log("plato lleno"); 
                 }
                 else{
                    InventoryItem plateItem = inventoryData.GetItemAt(0);
                    plateinventoryData.AddItem(plateItem);
                    DropItem(0, 1);
                    playerInventoryEmpty = false; 
                 }
            }
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
        }

        private void HandleTrashDescriptionRequest(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = trashinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                trashinventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            trashinventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
        }

        private void HandlePlateDescriptionRequest(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = plateinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                plateinventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            plateinventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
        }

        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                    $": {inventoryItem.itemState[i].value} / " +
                    $"{inventoryItem.item.DefaultParametersList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public InventoryItem GetInventoryFirstItem()
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(0);
            return inventoryItem;
        }

        public void TakeOutFirstItem()
        {
            inventoryData.RemoveItem(0, 1);
            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        public void AddInventoryItem(InventoryItem inventoryItem)
        {
            inventoryData.AddItem(inventoryItem);
        }

        public void Update()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen"){
                if (Input.GetKeyDown(KeyCode.I))
                {
                    if (!inventoryUI.isActiveAndEnabled)
                    {
                        inventoryUI.Show();
                        foreach (var item in inventoryData.GetCurrentInventoryState())
                        {
                            inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                        }
                    }
                    else
                    {
                        inventoryUI.Hide();
                    }
                }
            }
        }

        public void Testing(int itemIndex, string inventoryName)
        {
            name = inventoryName;
            lastDraggedIndex = itemIndex; 
        }
    }
}
