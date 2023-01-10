
using TextGameRPG.Scripts.Bot.Sessions;

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
        Scroll = 9,
    }

    static class ItemTypeExtensions
    {
        public static bool IsMultiSlot(this ItemType itemType)
        {
            return itemType == ItemType.Ring
                || itemType == ItemType.Scroll;
        }

        public static int GetSlotsCount(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Any:
                    return 0;
                case ItemType.Ring:
                    return 2;
                case ItemType.Scroll:
                    return 3;

                default:
                    return 1;
            }
        }

        public static bool IsSupportCommonRarity(this ItemType itemType)
        {
            return itemType != ItemType.Amulet
                && itemType != ItemType.Ring;
        }

        public static string GetCategoryLocalization(this ItemType category, GameSession session)
        {
            switch (category)
            {
                case ItemType.Equipped:
                    return Localizations.Localization.Get(session, $"menu_item_equipped");
                default:
                    string stringCategory = category.ToString().ToLower();
                    if (!stringCategory.EndsWith('s'))
                    {
                        stringCategory = stringCategory + 's';
                    }
                    return Localizations.Localization.Get(session, $"menu_item_{stringCategory}");
            }
        }

        public static string GetLocalization(this ItemType category, GameSession session)
        {
            return Localizations.Localization.Get(session, $"menu_item_{category.ToString().ToLower()}");
        }

    }    
}
