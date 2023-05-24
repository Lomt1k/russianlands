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
    protected static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();

    public string vendorCode { get; set; } = string.Empty;
    public ShopPriceBase? price { get; set; }

    public string GetPriceButtonView(GameSession session)
    {
        return price?.GetCompactView() ?? Localization.Get(session, "resource_price_free");
    }

    public virtual string GetNameForList(GameSession session)
    {
        var priceView = price?.GetCompactView() ?? Emojis.ElementWarningRed.ToString();
        return priceView + "\t" + GetTitle(session);
    }

    public virtual string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(GetTitle(session).Bold())
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_shop_item_view_contains_header"))
            .AppendLine(GetPossibleRewardsView(session));

        if (price != null)
        {
            sb.AppendLine(price.GetPriceView(session));
        }

        return sb.ToString();
    }

    public async Task TryPurchase(GameSession session, Func<Task> onSuccess, Func<Task> onFail)
    {
        var success = price == null ? true : await price.TryPurchase(session).FastAwait();
        if (success)
        {
            await GiveAndShowRewards(session, onSuccess).FastAwait();
        }
        else
        {
            await onFail().FastAwait();
        }
    }

    private async Task GiveAndShowRewards(GameSession session, Func<Task> onContinue)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_shop_purchased_items_header"));
        foreach (var reward in GetRewards())
        {
            var addedReward = await reward.AddReward(session).FastAwait();
            if (!string.IsNullOrEmpty(addedReward))
            {
                sb.AppendLine(addedReward);
            }
        }
        await notificationsManager.ShowNotification(session, sb, onContinue).FastAwait();
    }

    protected abstract string GetTitle(GameSession session);
    protected abstract string GetPossibleRewardsView(GameSession session);
    protected abstract IEnumerable<RewardBase> GetRewards();
    
}
