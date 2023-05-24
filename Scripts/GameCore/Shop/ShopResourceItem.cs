using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

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

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return new[] { resourceReward };
    }
}
