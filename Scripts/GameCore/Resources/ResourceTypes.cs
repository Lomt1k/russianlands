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
        FruitApple = 10,
        FruitPear = 11,
        FruitMandarin = 12,
        FruitCoconut = 13,
        FruitPineapple = 14,
        FruitBanana = 15,
        FruitWatermelon = 16,
        FruitStrawberry = 17,
        FruitBlueberry = 18,
        FruitKiwi = 19,
        FruitCherry = 20,
        FruitGrape = 21,
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

                ResourceType.FruitApple => Emojis.ResourceFruitApple,
                ResourceType.FruitPear => Emojis.ResourceFruitPear,
                ResourceType.FruitMandarin => Emojis.ResourceFruitMandarin,
                ResourceType.FruitCoconut => Emojis.ResourceFruitCoconut,
                ResourceType.FruitPineapple => Emojis.ResourceFruitPineapple,
                ResourceType.FruitBanana => Emojis.ResourceFruitBanana,
                ResourceType.FruitWatermelon => Emojis.ResourceFruitWatermelon,
                ResourceType.FruitStrawberry => Emojis.ResourceFruitStrawberry,
                ResourceType.FruitBlueberry => Emojis.ResourceFruitBlueberry,
                ResourceType.FruitKiwi => Emojis.ResourceFruitKiwi,
                ResourceType.FruitCherry => Emojis.ResourceFruitCherry,
                ResourceType.FruitGrape => Emojis.ResourceFruitGrape,

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

        public static bool IsFruit(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.FruitApple => true,
                ResourceType.FruitPear => true,
                ResourceType.FruitMandarin => true,
                ResourceType.FruitCoconut => true,
                ResourceType.FruitPineapple => true,
                ResourceType.FruitBanana => true,
                ResourceType.FruitWatermelon => true,
                ResourceType.FruitStrawberry => true,
                ResourceType.FruitBlueberry => true,
                ResourceType.FruitKiwi => true,
                ResourceType.FruitCherry => true,
                ResourceType.FruitGrape => true,
                _ => false
            };
        }

    }
}
