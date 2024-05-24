using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
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
        private UIInventoryPage trashinventoryUI; // Este es el inventario del basurero, aplicar misma lógica al refri.

        [SerializeField]
        private InventorySO inventoryData;

        [SerializeField]
        private InventorySO trashinventoryData;

        public List<InventoryItem> initialItems = new List<InventoryItem>();
        public List<InventoryItem> TrashInitialItems = new List<InventoryItem>();

        [SerializeField]
        private AudioClip dropClip;

        [SerializeField]
        private AudioSource audioSource;

        private void Start()
        {
            PrepareUI();
            PrepareInventoryData();

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Kitchen")
            {
                PrepareTrashInventoryData();
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

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
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

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleTrashDragging(int itemIndex)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            InventoryItem inventoryItem = trashinventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            trashinventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }

        private void HandleTrashSwapItems(int itemIndex_1, int itemIndex_2)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Kitchen") return;

            trashinventoryData.SwapItems(itemIndex_1, itemIndex_2);
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
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
                item.name, description);
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
            trashinventoryUI.UpdateDescription(itemIndex, item.ItemImage,
                item.name, description);
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

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!inventoryUI.isActiveAndEnabled)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState())
                    {
                        inventoryUI.UpdateData(item.Key,
                            item.Value.item.ItemImage,
                            item.Value.quantity);
                    }
                }
                else
                {
                    inventoryUI.Hide();
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                string sceneName = SceneManager.GetActiveScene().name;
                if (sceneName == "Kitchen")
                {
                    if (!trashinventoryUI.isActiveAndEnabled)
                    {
                        trashinventoryUI.Show();
                        foreach (var item in trashinventoryData.GetCurrentInventoryState())
                        {
                            trashinventoryUI.UpdateData(item.Key,
                                item.Value.item.ItemImage,
                                item.Value.quantity);
                        }
                    }
                    else
                    {
                        trashinventoryUI.Hide();
                    }
                }
            }
        }
    }
}
