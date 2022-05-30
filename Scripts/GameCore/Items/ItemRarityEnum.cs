
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public enum ItemRarity : byte
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3
    }

    public static class ItemRarityExtensions
    {
        public static string GetView(this ItemRarity rarity, GameSession session)
        {
            return Localization.Localization.Get(session, $"item_rarity_{rarity.ToString().ToLower()}");
        }
    }
}
