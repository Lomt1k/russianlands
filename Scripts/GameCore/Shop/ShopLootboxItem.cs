using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopLootboxItem : ShopItemBase
{
    public string titleLocalizationKey { get; set; } = string.Empty;
    public List<RewardBase> rewards { get; set; } = new();

    protected override string GetTitle(GameSession session)
    {
        return Localization.Get(session, titleLocalizationKey);
    }

    protected override string GetPossibleRewardsView(GameSession session)
    {
        return rewards.GetPossibleRewardsView(session);
    }    

    protected override async Task GiveAndShowRewards(GameSession session, Func<Task> onContinue)
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
