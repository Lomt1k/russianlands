using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Inventory
{
    public class PlayerInventory
    {
        public const int DEFAULT_SIZE = 50;

        public int inventorySize { get; private set; } = DEFAULT_SIZE;
        public List<InventoryItem> items { get; private set; } = new List<InventoryItem>(DEFAULT_SIZE);

        [JsonIgnore]
        public EquippedItems equipped { get; }

        [JsonIgnore]
        public bool isFull => items.Count >= inventorySize;

        [JsonConstructor]
        public PlayerInventory()
        {
            equipped = new EquippedItems(this);
        }

        public List<InventoryItem> GetAllItems()
        {
            return items;
        }

        public IEnumerable<InventoryItem> GetItemsByType(ItemType type)
        {
            return items.Where(x => x.data.itemType == type);
        }

        public InventoryItem? TryAddItem(int itemId)
        {
            if (isFull)
                return null;

            var item = new InventoryItem(itemId);
            items.Add(item);
            return item;
        }

        public InventoryItem? TryAddItem(InventoryItem item)
        {
            if (isFull)
                return null;

            items.Add(item);
            return item;
        }

        public void RemoveItem(InventoryItem item)
        {
            if (item.isEquipped)
            {
                equipped.Unequip(item);
            }
            items.Remove(item);
        }

        public void EquipSingleSlot(InventoryItem item)
        {
            equipped.EquipSingleSlot(item);
        }

        public void EquipMultiSlot(InventoryItem item, int slot)
        {
            equipped.EquipMultiSlot(item, slot);
        }

        public void Unequip(InventoryItem item)
        {
            equipped.Unequip(item);
        }

        public void AddNewItemSlots(int count)
        {
            if (count < 1)
                return;

            inventorySize += count;
        }

    }
}
