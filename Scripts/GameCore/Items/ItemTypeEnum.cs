
namespace TextGameRPG.Scripts.GameCore.Items
{
    public enum ItemType : sbyte
    {
        //for category! Types with negative values not used in item data
        Any = -1,
        Equipped = -2,

        //normal item types
        Sword = 0,
        Bow = 1,
        Stick = 2,
        Armor = 3,
        Helmet = 4,
        Boots = 5,
        Shield = 6,
        Amulet = 7,
        Ring = 8,        
        Poison = 9,
        Tome = 10,
        Scroll = 11
    }

    static class ItemTypeExtensions
    {
        public static bool IsMultiSlot(this ItemType itemType)
        {
            return itemType == ItemType.Ring
                || itemType == ItemType.Poison
                || itemType == ItemType.Tome;
        }

        public static int GetSlotsCount(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Any:
                    return 0;
                case ItemType.Ring:
                    return 2;
                case ItemType.Poison:
                case ItemType.Tome:
                    return 3;
                case ItemType.Scroll:
                    return 0;

                default:
                    return 1;
            }
        }

        public static bool IsEquippable(this ItemType itemType)
        {
            return itemType != ItemType.Scroll;
        }
    }    
}
