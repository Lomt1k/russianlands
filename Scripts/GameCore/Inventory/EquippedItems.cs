using System;
using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Inventory
{
    public class EquippedItems
    {
        private Dictionary<ItemType, InventoryItem?> _singleEquipped = new Dictionary<ItemType, InventoryItem?>
        {
            { ItemType.Sword, null },
            { ItemType.Bow, null },
            { ItemType.Stick, null },
            { ItemType.Helmet, null },
            { ItemType.Armor, null },
            { ItemType.Boots, null },
            { ItemType.Shield, null },
            { ItemType.Amulet, null }
        };
        private Dictionary<ItemType, InventoryItem?[]> _multiEquipped = new Dictionary<ItemType, InventoryItem?[]>
        {
            { ItemType.Ring, new InventoryItem?[ItemType.Ring.GetSlotsCount()] },
            { ItemType.Poison, new InventoryItem?[ItemType.Poison.GetSlotsCount()] },
            { ItemType.Scroll, new InventoryItem?[ItemType.Scroll.GetSlotsCount()] },
        };

        private List<InventoryItem> _allEquipped = new List<InventoryItem>();

        public InventoryItem[] allEquipped => _allEquipped.ToArray();

        public InventoryItem? this[ItemType type] => _singleEquipped[type];

        public InventoryItem? this[ItemType type, int slot] => _multiEquipped[type][slot];

        public Action? onUpdateEquippedItems;

        public EquippedItems(PlayerInventory inventory)
        {
            var allEquipped = inventory.GetAllItems().Where(x => x.isEquipped).OrderBy(x => x.data.itemType);
            foreach (var item in allEquipped)
            {
                TrySetupAsEquipped(item);
            }
        }

        // в том числе исправляет "лишние" экипированные предметы в случае некорректной даты
        private void TrySetupAsEquipped(InventoryItem item)
        {
            var itemType = item.data.itemType;
            if (_multiEquipped.ContainsKey(itemType))
            {
                for (int i = 0; i < _multiEquipped[itemType].Length; i++)
                {
                    if (_multiEquipped[itemType][i] == null)
                    {
                        _multiEquipped[itemType][i] = item;
                        _allEquipped.Add(item);
                        return;
                    }
                }
                item.SetEquippedState(false);
            }
            else
            {
                if (!_singleEquipped.ContainsKey(itemType))
                {
                    _singleEquipped.Add(itemType, item);
                    _allEquipped.Add(item);
                }
                else
                {
                    item.SetEquippedState(false);
                }
            }
        }

        public void EquipSingleSlot(InventoryItem item)
        {
            var itemType = item.data.itemType;
            if (itemType.IsMultiSlot())
            {
                Program.logger.Error($"Inventory: Can not equip item with type '{itemType}' in single slot");
                return;
            }

            var currentItem = _singleEquipped[itemType];
            if (currentItem != null)
            {
                Unequip(currentItem, withInvokeEvent: false);
            }

            _singleEquipped[itemType] = item;
            InsertToAllEquippedWithItemTypeOrdering(item);
            item.SetEquippedState(true);
            OnUpdateEquippedItems();
        }

        public void EquipMultiSlot(InventoryItem item, int slot)
        {
            var itemType = item.data.itemType;
            if (!itemType.IsMultiSlot())
            {
                Program.logger.Error($"Inventory: Can not equip item with type '{itemType}' in multi slot");
                return;
            }

            var currentItem = _multiEquipped[itemType][slot];
            if (currentItem != null)
            {
                Unequip(currentItem, withInvokeEvent: false);
            }

            _multiEquipped[itemType][slot] = item;
            InsertToAllEquippedWithItemTypeOrdering(item);
            item.SetEquippedState(true);
            OnUpdateEquippedItems();
        }

        private void InsertToAllEquippedWithItemTypeOrdering(InventoryItem item)
        {
            int index = _allEquipped.Count;
            for (int i = 0; i < _allEquipped.Count; i++)
            {
                if ((sbyte)_allEquipped[i].data.itemType > (sbyte)item.data.itemType)
                {
                    index = i;
                    break;
                }
            }
            _allEquipped.Insert(index, item);
        }

        public void Unequip(InventoryItem item, bool withInvokeEvent = true)
        {
            var itemType = item.data.itemType;
            if (itemType.IsMultiSlot())
            {
                for (int i = 0; i < _multiEquipped[itemType].Length; i++)
                {
                    if (_multiEquipped[itemType][i] == item)
                    {
                        _multiEquipped[itemType][i] = null;
                        break;
                    }
                }
            }
            else if (_singleEquipped[itemType] == item)
            {
                _singleEquipped[itemType] = null;
            }

            _allEquipped.Remove(item);
            item.SetEquippedState(false);

            if (withInvokeEvent)
            {
                OnUpdateEquippedItems();
            }
        }

        private void OnUpdateEquippedItems()
        {
            onUpdateEquippedItems?.Invoke();
        }

        
    }
}
