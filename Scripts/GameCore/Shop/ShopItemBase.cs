using JsonKnownTypes;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<ShopItemBase>))]
public abstract class ShopItemBase
{
    private static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();

    public string localizationKey { get; set; } = string.Empty;
    public List<RewardBase> rewards { get; set; } = new();
    public ShopPriceBase? price { get; set; }

    public string GetPriceButtonView(GameSession session)
    {
        return price?.GetCompactView() ?? Localization.Get(session, "resource_price_free");
    }

    public virtual string GetNameForList(GameSession session)
    {
        var priceView = price?.GetCompactView() ?? Emojis.ElementWarningRed.ToString();
        var itemName = Localization.Get(session, localizationKey);
        return priceView + "\t" + itemName;
    }

    public virtual string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, localizationKey).Bold())
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_shop_item_view_contains_header"))
            .AppendLine(GetRewardsView(session));

        if (price != null)
        {
            sb.AppendLine(price.GetPriceView(session));
        }

        return sb.ToString();
    }

    private string GetRewardsView(GameSession session)
    {
        var sb = new StringBuilder();
        foreach (var reward in rewards)
        {
            sb.AppendLine(reward.GetPossibleRewardsView(session));
        }
        return sb.ToString();
    }

    public async Task TryPurchase(GameSession session, Func<Task> onSuccess, Func<Task> onFail)
    {
        var success = price == null ? true : await price.TryPurchase(session).FastAwait();
        if (success)
        {
            await GiveRewards(session, onSuccess).FastAwait();
        }
        else
        {
            await onFail().FastAwait();
        }
    }

    protected virtual async Task GiveRewards(GameSession session, Func<Task> onContinue)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_shop_purchased_items_header"));
        foreach (var reward in rewards)
        {
            var addedReward = await reward.AddReward(session).FastAwait();
            if (!string.IsNullOrEmpty(addedReward))
            {
                sb.AppendLine(addedReward);
            }
        }
        await notificationsManager.ShowNotification(session, sb, onContinue).FastAwait();
    }

}
