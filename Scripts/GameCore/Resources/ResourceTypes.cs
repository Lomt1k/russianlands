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
