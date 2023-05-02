using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Resources;

public record struct ResourceData(ResourceId resourceId, int amount);

public static class ResourceDataExtensions
{
    public static string GetCompactView(this ResourceData resourceData)
    {
        return resourceData.resourceId.GetEmoji() + resourceData.amount.ShortView();
    }

    public static string GetCompactView(this IEnumerable<ResourceData> resourceDatas)
    {
        var sb = new StringBuilder();
        var elementsInCurrentRow = 0;
        foreach (var resourceData in resourceDatas)
        {
            if (elementsInCurrentRow == 3)
            {
                sb.AppendLine();
                elementsInCurrentRow = 0;
            }
            if (elementsInCurrentRow > 0)
            {
                sb.Append(Emojis.bigSpace);
            }

            sb.Append(resourceData.GetCompactView());
            elementsInCurrentRow++;
        }

        return sb.ToString();
    }

    public static string GetLocalizedView(this ResourceData resourceData, GameSession session)
    {
        var localizationKey = "resource_name_" + resourceData.resourceId.ToString().ToLower();
        return resourceData.resourceId.GetEmoji() + Localization.Get(session, localizationKey) + $" {resourceData.amount.View()}";
    }

    public static string GetLocalizedView(this IEnumerable<ResourceData> resourceDatas, GameSession session)
    {
        var sb = new StringBuilder();
        foreach (var resourceData in resourceDatas)
        {
            sb.AppendLine(resourceData.GetLocalizedView(session));
        }
        return sb.ToString();
    }

    public static string GetPriceView(this ResourceData resourceData, GameSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "resource_header_price"));

        sb.AppendLine(resourceData.amount > 0
            ? resourceData.GetLocalizedView(session)
            : Localization.Get(session, "resource_price_free"));

        return sb.ToString();
    }

    public static string GetPriceView(this IEnumerable<ResourceData> resourceDatas, GameSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "resource_header_price"));

        var hasAmount = false;
        foreach (var resourceData in resourceDatas)
        {
            if (resourceData.amount < 1)
                continue;

            hasAmount = true;
            sb.AppendLine(resourceData.GetLocalizedView(session));
        }

        if (!hasAmount)
        {
            sb.AppendLine(Localization.Get(session, "resource_price_free"));
        }

        return sb.ToString();
    }

}
