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
        public bool isFull => items.Count >= inventorySize;

        [JsonConstructor]
        public PlayerInventory()
        {
        }

        public List<InventoryItem> GetAllItems()
        {
            return items;
        }

        public List<InventoryItem> GetItemsByType(ItemType type)
        {
            return items.Where(x => x.data.itemType == type).ToList();
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
            items.Remove(item);
        }

    }
}
