using MarkOne.Scripts.Bot;

namespace MarkOne.Scripts.GameCore.Resources;

public enum ResourceId : byte
{
    Gold = 0,
    Food = 1,
    Diamond = 2,
    Herbs = 3,
    Wood = 4,
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
    CrossroadsEnergy = 22,
    ArenaTicket = 23,
    ArenaChip = 24,
}

public static class ResourceIdExtensions
{
    public static Emoji GetEmoji(this ResourceId resourceId)
    {
        return resourceId switch
        {
            ResourceId.Gold => Emojis.ResourceGold,
            ResourceId.Food => Emojis.ResourceFood,
            ResourceId.Diamond => Emojis.ResourceDiamond,
            ResourceId.Herbs => Emojis.ResourceHerbs,
            ResourceId.Wood => Emojis.ResourceWood,

            ResourceId.InventoryItems => Emojis.ResourceInventoryItems,
            ResourceId.CraftPiecesCommon => Emojis.ResourceCraftPiecesCommon,
            ResourceId.CraftPiecesRare => Emojis.ResourceCraftPiecesRare,
            ResourceId.CraftPiecesEpic => Emojis.ResourceCraftPiecesEpic,
            ResourceId.CraftPiecesLegendary => Emojis.ResourceCraftPiecesLegendary,

            ResourceId.FruitApple => Emojis.ResourceFruitApple,
            ResourceId.FruitPear => Emojis.ResourceFruitPear,
            ResourceId.FruitMandarin => Emojis.ResourceFruitMandarin,
            ResourceId.FruitCoconut => Emojis.ResourceFruitCoconut,
            ResourceId.FruitPineapple => Emojis.ResourceFruitPineapple,
            ResourceId.FruitBanana => Emojis.ResourceFruitBanana,
            ResourceId.FruitWatermelon => Emojis.ResourceFruitWatermelon,
            ResourceId.FruitStrawberry => Emojis.ResourceFruitStrawberry,
            ResourceId.FruitBlueberry => Emojis.ResourceFruitBlueberry,
            ResourceId.FruitKiwi => Emojis.ResourceFruitKiwi,
            ResourceId.FruitCherry => Emojis.ResourceFruitCherry,
            ResourceId.FruitGrape => Emojis.ResourceFruitGrape,

            ResourceId.CrossroadsEnergy => Emojis.ResourceCrossroadsEnergy,
            ResourceId.ArenaTicket => Emojis.ResourceArenaTicket,
            ResourceId.ArenaChip => Emojis.ResourceArenaChip,

            _ => Emojis.Empty
        };
    }

    public static bool IsCraftResource(this ResourceId resourceId)
    {
        return resourceId == ResourceId.CraftPiecesCommon
            || resourceId == ResourceId.CraftPiecesRare
            || resourceId == ResourceId.CraftPiecesEpic
            || resourceId == ResourceId.CraftPiecesLegendary;
    }

    public static bool IsFruit(this ResourceId resourceId)
    {
        return resourceId switch
        {
            ResourceId.FruitApple => true,
            ResourceId.FruitPear => true,
            ResourceId.FruitMandarin => true,
            ResourceId.FruitCoconut => true,
            ResourceId.FruitPineapple => true,
            ResourceId.FruitBanana => true,
            ResourceId.FruitWatermelon => true,
            ResourceId.FruitStrawberry => true,
            ResourceId.FruitBlueberry => true,
            ResourceId.FruitKiwi => true,
            ResourceId.FruitCherry => true,
            ResourceId.FruitGrape => true,
            _ => false
        };
    }

}
