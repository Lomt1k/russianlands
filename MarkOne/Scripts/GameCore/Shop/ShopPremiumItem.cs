using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;
using System.Text;

namespace MarkOne.Scripts.GameCore.Shop;
public class ShopPremiumItem : ShopItemBase
{
    public PremiumReward premiumReward { get; set; } = new();

    public override string GetTitle(GameSession session)
    {
        return premiumReward.GetPossibleRewardsView(session);
    }

    public override string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(GetTitle(session));

        if (price != null)
        {
            sb.AppendLine();
            sb.AppendLine(price.GetPlayerResourcesView(session));
        }

        return sb.ToString();
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return new[] { premiumReward };
    }

}
