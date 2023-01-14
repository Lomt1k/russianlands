using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using TextGameRPG.Scripts.Bot.Sessions;
using System;
using System.Linq;

namespace TextGameRPG.Scripts.GameCore.Inventory
{
    [JsonObject]
    public class PlayerInventory
    {
        [JsonProperty]
        public List<InventoryItem> items { get; private set; } = new List<InventoryItem>();

        [JsonIgnore]
        private readonly Dictionary<ItemType, List<InventoryItem>> _itemsByType = new Dictionary<ItemType, List<InventoryItem>>();
        [JsonIgnore]
        private readonly Dictionary<ItemType, bool> _hasNewItemsInCategory = new Dictionary<ItemType, bool>();
        [JsonIgnore]
        private bool _hasAnyNewItem; 
      
        [JsonIgnore]
        public EquippedItems equipped { get; private set; }
        [JsonIgnore]
        public GameSession session { get; private set; }
        [JsonIgnore]
        public bool isFull => items.Count >= inventorySize;
        [JsonIgnore]
        public int itemsCount => items.Count;
        [JsonIgnore]
        public int inventorySize => session.player.resources.GetResourceLimit(Resources.ResourceType.InventoryItems);
        [JsonIgnore]
        public bool hasAnyNewItem => _hasAnyNewItem;

        public void SetupSession(GameSession _session)
        {
            session = _session;
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            equipped = new EquippedItems(this);
            SortItemsByType();
            UpdateHasNewItemsState();
        }

        private void SortItemsByType()
        {
            var itemTypes = Enum.GetValues(typeof(ItemType));
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

        public IEnumerable<InventoryItem> GetItemsByType(ItemType type)
        {
            return _itemsByType[type];
        }

        public int GetItemsCountByType(ItemType type)
        {
            return _itemsByType[type].Count;
        }

        public bool TryAddItem(InventoryItem item)
        {
            if (isFull)
                return false;

            items.Add(item);
            _itemsByType[item.data.itemType].Add(item);
            if (item.isNew)
            {
                _hasNewItemsInCategory[item.data.itemType] = true;
                _hasAnyNewItem = true;
            }
            return true;
        }

        public bool ForceAddItem(InventoryItem item)
        {
            items.Add(item);
            _itemsByType[item.data.itemType].Add(item);
            if (item.isNew)
            {
                _hasNewItemsInCategory[item.data.itemType] = true;
                _hasAnyNewItem = true;
            }
            return true;
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

        public bool HasNewInCategory(ItemType category)
        {
            return _hasNewItemsInCategory[category];
        }

        public void UpdateHasNewItemsState()
        {
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                if (itemType < 0)
                    continue;

                _hasNewItemsInCategory[itemType] = _itemsByType[itemType].Any(x => x.isNew);
            }
            _hasAnyNewItem = _hasNewItemsInCategory.Any(x => x.Value == true);
        }

    }
}
