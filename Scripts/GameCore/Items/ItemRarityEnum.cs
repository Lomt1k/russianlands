using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Items;

public enum Rarity : byte
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3
}

public static class ItemRarityExtensions
{
    public static string GetView(this Rarity rarity, GameSession session)
    {
        return Localizations.Localization.Get(session, $"item_rarity_{rarity.ToString().ToLower()}");
    }

    public static int GetKeywordsCount(this Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Epic => 1,
            Rarity.Legendary => 2,
            _ => 0
        };
    }

}
