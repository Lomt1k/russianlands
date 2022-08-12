using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public enum ResourceType : byte
    {
        //General
        Gold = 0,
        Food = 1,
        Diamonds = 2,
        Wood = 3,
        Iron = 4,

        //Others
        Arrows = 255
    }

    public static class ResourceTypeExtensions
    {
        public static string GetShortView(this ResourceType resourceType, int amount)
        {
            return $"{Emojis.resources[resourceType]} {amount.ShortView()}";
        }

        public static string GetLocalizedView(this ResourceType resourceType, GameSession session, int amount)
        {
            var localizationKey = "resource_name_" + resourceType.ToString().ToLower();
            return $"{Emojis.resources[resourceType]} {Localization.Get(session, localizationKey)} {amount.View()}";
        }
    }
}
