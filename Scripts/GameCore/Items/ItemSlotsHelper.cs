using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public enum ItemSlot
    {
        MeleeWeapon = 0,
        Bow = 1,
        Stick = 2,
        Armour = 3,
        Helmet = 4,
        Gloves = 5,
        Shoes = 6,
        FirstRing = 7,
        SecondRing = 8,
        Amulet = 9
    }

    public static class ItemSlotsHelper
    {
        private static Dictionary<ItemSlot, ItemType> _itemTypeBySlotDictionary;

        static ItemSlotsHelper()
        {
            InitializeItemTypeDictionary();
        }

        private static void InitializeItemTypeDictionary()
        {
            _itemTypeBySlotDictionary = new Dictionary<ItemSlot, ItemType>
            {
                {ItemSlot.MeleeWeapon, ItemType.MeleeWeapon },
                {ItemSlot.Bow, ItemType.Bow },
                {ItemSlot.Stick, ItemType.Stick },
                {ItemSlot.Armour, ItemType.Armour },
                {ItemSlot.Helmet, ItemType.Helmet },
                {ItemSlot.Gloves, ItemType.Gloves },
                {ItemSlot.Shoes, ItemType.Shoes },
                {ItemSlot.FirstRing, ItemType.Ring },
                {ItemSlot.SecondRing, ItemType.Ring },
                {ItemSlot.Amulet, ItemType.Amulet }
            };
        }

        public static ItemType GetItemTypeBySlot(ItemSlot slot)
        {
            return _itemTypeBySlotDictionary[slot];
        }


    }
}
