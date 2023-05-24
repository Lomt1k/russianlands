using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopInventoryItem : ShopItemBase
{
    public ItemWithCodeReward reward { get; set; } = new();

    protected override string GetTitle(GameSession session)
    {
        return reward.itemTemplate.GetFullName(session);
    }

    public override string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(reward.itemTemplate.GetView(session));

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
        var addedReward = await reward.AddReward(session).FastAwait();
        if (!string.IsNullOrEmpty(addedReward))
        {
            sb.AppendLine(addedReward);
        }
        await notificationsManager.ShowNotification(session, sb, onContinue).FastAwait();
    }
}
