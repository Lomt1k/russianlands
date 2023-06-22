using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public record struct ResourceData(ResourceId resourceId, int amount);

public static class ResourceDataExtensions
{
    public static string GetCompactView(this ResourceData resourceData, bool shortView = true)
    {
        return shortView ? resourceData.resourceId.GetEmoji() + resourceData.amount.ShortView()
            : resourceData.resourceId.GetEmoji() + resourceData.amount.View();
    }

    public static string GetCompactView(this IEnumerable<ResourceData> resourceDatas, bool shortView = true)
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

            sb.Append(resourceData.GetCompactView(shortView));
            elementsInCurrentRow++;
        }

        return sb.ToString();
    }

    public static string GetLocalizedView(this ResourceData resourceData, GameSession session, bool showCountIfSingle = true)
    {
        return resourceData.GetLocalizedView(session.language, showCountIfSingle);
    }

    public static string GetLocalizedView(this IEnumerable<ResourceData> resourceDatas, GameSession session, bool showCountIfSingle = true)
    {
        return resourceDatas.GetLocalizedView(session.language, showCountIfSingle);
    }

    public static string GetLocalizedView(this ResourceData resourceData, LanguageCode languageCode, bool showCountIfSingle = true)
    {
        var localizationKey = "resource_name_" + resourceData.resourceId.ToString().ToLower();
        return resourceData.amount == 1 && !showCountIfSingle
            ? resourceData.resourceId.GetEmoji() + Localization.Get(languageCode, localizationKey).Bold()
            : resourceData.resourceId.GetEmoji() + (Localization.Get(languageCode, localizationKey) + ':').Bold() + $" {resourceData.amount.View()}";
    }

    public static string GetLocalizedView(this IEnumerable<ResourceData> resourceDatas, LanguageCode languageCode, bool showCountIfSingle = true)
    {
        var sb = new StringBuilder();
        foreach (var resourceData in resourceDatas)
        {
            sb.AppendLine(resourceData.GetLocalizedView(languageCode, showCountIfSingle));
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
