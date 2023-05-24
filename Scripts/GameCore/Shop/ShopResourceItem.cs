using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopResourceItem : ShopItemBase
{
    public ResourceReward resourceReward { get; set; } = new();

    protected override string GetTitle(GameSession session)
    {
        return resourceReward.resourceData.GetCompactView(shortView: false);
    }

    public override string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(GetTitle(session));

        if (price != null)
        {
            sb.AppendLine(price.GetPriceView(session));
        }

        return sb.ToString();
    }

    protected override string GetPossibleRewardsView(GameSession session)
    {
        // not used
        return string.Empty;
    }

    protected override async Task GiveAndShowRewards(GameSession session, Func<Task> onContinue)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_shop_purchased_items_header"));
        var addedReward = await resourceReward.AddReward(session).FastAwait();
        if (!string.IsNullOrEmpty(addedReward))
        {
            sb.AppendLine(addedReward);
        }
        await notificationsManager.ShowNotification(session, sb, onContinue).FastAwait();
    }
}
