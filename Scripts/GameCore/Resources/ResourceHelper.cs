using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public static class ResourceHelper
    {
        public static string GetPriceView(GameSession session, Dictionary<ResourceType,int> resources)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "resource_price_header"));

            bool hasAmount = false;
            foreach (var kvp in resources)
            {
                var resourceType = kvp.Key;
                var resourceAmount = kvp.Value;
                if (resourceAmount < 1)
                    continue;

                hasAmount = true;
                sb.AppendLine(resourceType.GetLocalizedView(session, resourceAmount));
            }

            if (!hasAmount)
            {
                sb.AppendLine(Localization.Get(session, "resource_price_free"));
            }

            return sb.ToString();
        }


    }
}
