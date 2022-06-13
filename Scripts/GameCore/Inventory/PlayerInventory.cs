using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TextGameRPG.Scripts.GameCore.Inventory
{
    public class PlayerInventory
    {
        public const int DEFAULT_SIZE = 50;

        private readonly Dictionary<ItemType, List<InventoryItem>> _itemsByType = new Dictionary<ItemType, List<InventoryItem>>();

        public int inventorySize { get; private set; } = DEFAULT_SIZE;
        public List<InventoryItem> items { get; private set; } = new List<InventoryItem>(DEFAULT_SIZE);

        [JsonIgnore]
        public EquippedItems? equipped { get; private set; }

        [JsonIgnore]
        public bool isFull => items.Count >= inventorySize;

        [JsonIgnore]
        public int itemsCount => items.Count;

        private PlayerInventory()
        {
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            equipped = new EquippedItems(this);
            SortItemsByType();
        }

        private void SortItemsByType()
        {
            var itemTypes = System.Enum.GetValues(typeof(ItemType));
            foreach (var element in itemTypes)
            {
                var itemType = (ItemType)element;
                if (itemType < 0)
                    continue;

                _itemsByType[itemType] = new List<InventoryItem>();
            }

            foreach (var item in items)
            {
                if (item == null)
                    continue;

                var itemType = item.data.itemType;
                if (!_itemsByType.ContainsKey(itemType))
                {
                    _itemsByType.Add(itemType, new List<InventoryItem>());
                }
                var index = item.isEquipped ? 0 : _itemsByType[itemType].Count;
                _itemsByType[itemType].Insert(index, item);
            }
        }

        public List<InventoryItem> GetAllItems()
        {
            return items;
        }

        public IEnumerable<ItemType> GetItemTypes()
        {
            return _itemsByType.Keys;
        }

        public IEnumerable<InventoryItem> GetItemsByType(ItemType type)
        {
            return _itemsByType[type];
        }

        public int GetItemsCountByType(ItemType type)
        {
            return _itemsByType[type].Count;
        }

        public InventoryItem? TryAddItem(int itemId)
        {
            if (isFull)
                return null;

            if (!GameDataBase.GameDataBase.instance.items.ContainsKey(itemId))
                return null;

            var item = new InventoryItem(itemId);
            items.Add(item);
            _itemsByType[item.data.itemType].Add(item);
            return item;
        }

        public InventoryItem? TryAddItem(InventoryItem item)
        {
            if (isFull)
                return null;

            items.Add(item);
            _itemsByType[item.data.itemType].Add(item);
            return item;
        }

        public void RemoveItem(InventoryItem item)
        {
            if (item.isEquipped)
            {
                equipped.Unequip(item);
            }
            items.Remove(item);
            _itemsByType[item.data.itemType].Remove(item);
        }

        // Можно экипировать даже предмет, который не находится в инвентаре! Не баг, а фича
        public void EquipSingleSlot(InventoryItem item)
        {
            equipped.EquipSingleSlot(item);
            SetFirstIndexForItem(item);
        }

        // Можно экипировать даже предмет, который не находится в инвентаре! Не баг, а фича
        public void EquipMultiSlot(InventoryItem item, int slot)
        {
            equipped.EquipMultiSlot(item, slot);
            SetFirstIndexForItem(item);
        }

        public void Unequip(InventoryItem item)
        {
            equipped.Unequip(item);
            SetFirstUnequippedIndexForItem(item);
        }

        private void SetFirstIndexForItem(InventoryItem item)
        {
            _itemsByType[item.data.itemType].Remove(item);
            _itemsByType[item.data.itemType].Insert(0, item);
        }

        private void SetFirstUnequippedIndexForItem(InventoryItem item)
        {
            var itemType = item.data.itemType;
            _itemsByType[itemType].Remove(item);
            int index = 0;
            for (int i = 0; i < _itemsByType[itemType].Count; i++)
            {
                if (!_itemsByType[itemType][i].isEquipped)
                {
                    index = i;
                    break;
                }
            }
            _itemsByType[itemType].Insert(index, item);
        }

        public void AddNewItemSlots(int count)
        {
            if (count < 1)
                return;

            inventorySize += count;
        }

    }
}
