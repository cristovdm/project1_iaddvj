using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> inventoryItems;

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>(new InventoryItem[Size]);
            for (int i = 0; i < Size; i++)
            {
                inventoryItems[i] = InventoryItem.GetEmptyItem();
            }
            InformAboutChange();
        }

        public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            if (item == null)
            {
                Debug.LogError("Item es null en AddItem.");
                return quantity;
            }

            if (!item.isStackable)
            {
                while (quantity > 0 && !IsInventoryFull())
                {
                    quantity -= AddItemToFirstFreeSlot(item, 1, itemState);
                }
                InformAboutChange();
                return quantity;
            }

            quantity = AddStackableItem(item, quantity, itemState);
            InformAboutChange();
            return quantity;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            if (item == null)
            {
                Debug.LogError("Item es null en AddItemToFirstFreeSlot.");
                return 0;
            }

            InventoryItem newItem = new InventoryItem
            {
                item = item,
                quantity = quantity,
                itemState = itemState != null ? new List<ItemParameter>(itemState) : new List<ItemParameter>(item.DefaultParametersList)
            };

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = newItem;
                    return quantity;
                }
            }
            return 0;
        }

        public bool IsInventoryFull() => inventoryItems.All(item => !item.IsEmpty);

        private int AddStackableItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            if (item == null)
            {
                Debug.LogError("Item es null en AddStackableItem.");
                return quantity;
            }

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty || inventoryItems[i].item.ID != item.ID)
                    continue;

                int amountPossibleToTake = inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;
                int amountToAdd = Math.Min(quantity, amountPossibleToTake);

                inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + amountToAdd);
                quantity -= amountToAdd;

                if (quantity == 0)
                {
                    InformAboutChange();
                    return 0;
                }
            }

            while (quantity > 0 && !IsInventoryFull())
            {
                int amountToAdd = Math.Min(quantity, item.MaxStackSize);
                quantity -= amountToAdd;
                AddItemToFirstFreeSlot(item, amountToAdd, itemState);
            }
            return quantity;
        }

        public void RemoveItem(int itemIndex, int amount)
        {
            if (itemIndex < 0 || itemIndex >= inventoryItems.Count)
                throw new IndexOutOfRangeException("Índice fuera de los límites.");

            if (inventoryItems[itemIndex].IsEmpty)
                return;

            int remaining = inventoryItems[itemIndex].quantity - amount;
            if (remaining <= 0)
                inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
            else
                inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(remaining);

            InformAboutChange();
        }

        public void AddItem(InventoryItem item)
        {
            if (item.item == null)
            {
                Debug.LogError("El InventoryItem que se intenta agregar tiene un item null.");
                return;
            }
            AddItem(item.item, item.quantity, item.itemState);
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            return inventoryItems
                .Select((item, index) => new { item, index })
                .Where(x => !x.item.IsEmpty)
                .ToDictionary(x => x.index, x => x.item);
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= inventoryItems.Count)
                throw new IndexOutOfRangeException("Índice fuera de los límites.");

            return inventoryItems[itemIndex];
        }

        public bool SwapItems(int itemIndex1, int itemIndex2)
        {
            if (itemIndex1 < 0 || itemIndex1 >= inventoryItems.Count ||
                itemIndex2 < 0 || itemIndex2 >= inventoryItems.Count)
            {
                return false;
            }

            InventoryItem temp = inventoryItems[itemIndex1];
            inventoryItems[itemIndex1] = inventoryItems[itemIndex2];
            inventoryItems[itemIndex2] = temp;

            InformAboutChange();
            return true;
        }

        public void ClearInventory()
        {
            Initialize();
        }

        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }

    [Serializable]
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;
        public List<ItemParameter> itemState;
        public bool IsEmpty => item == null;

        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
                itemState = new List<ItemParameter>(this.itemState)
            };
        }

        public static InventoryItem GetEmptyItem() => new InventoryItem
        {
            item = null,
            quantity = 0,
            itemState = new List<ItemParameter>()
        };
    }
}
