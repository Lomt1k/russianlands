using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public enum ResourceType : byte
    {
        //General
        Gold = 0,
        Food = 1,
        Diamond = 2,
        Herbs = 3,
        Wood = 4,

        //Others
        InventoryItems = 5, // регулирует размер инвентаря
        CraftPiecesCommon = 6,
        CraftPiecesRare = 7,
        CraftPiecesEpic = 8,
        CraftPiecesLegendary = 9,
    }

    public static class ResourceTypeExtensions
    {
        public static Emoji GetEmoji(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Gold => Emojis.ResourceGold,
                ResourceType.Food => Emojis.ResourceFood,
                ResourceType.Diamond => Emojis.ResourceDiamond,
                ResourceType.Herbs => Emojis.ResourceHerbs,
                ResourceType.Wood => Emojis.ResourceWood,

                ResourceType.InventoryItems => Emojis.ResourceInventoryItems,
                ResourceType.CraftPiecesCommon => Emojis.ResourceCraftPiecesCommon,
                ResourceType.CraftPiecesRare => Emojis.ResourceCraftPiecesRare,
                ResourceType.CraftPiecesEpic => Emojis.ResourceCraftPiecesEpic,
                ResourceType.CraftPiecesLegendary => Emojis.ResourceCraftPiecesLegendary,

                _ => Emojis.Empty
            };
        }

        public static string GetShortView(this ResourceType resourceType, int amount)
        {
            return GetEmoji(resourceType) + amount.ShortView();
        }

        public static string GetLocalizedView(this ResourceType resourceType, GameSession session, int amount)
        {
            var localizationKey = "resource_name_" + resourceType.ToString().ToLower();
            return GetEmoji(resourceType) + Localization.Get(session, localizationKey) + $" {amount.View()}";
        }

        public static bool IsCraftResource(this ResourceType resourceType)
        {
            return resourceType == ResourceType.CraftPiecesCommon
                || resourceType == ResourceType.CraftPiecesRare
                || resourceType == ResourceType.CraftPiecesEpic
                || resourceType == ResourceType.CraftPiecesLegendary;
        }
    }
}
