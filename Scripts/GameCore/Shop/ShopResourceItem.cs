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

    public override string GetTitle(GameSession session)
    {
        return resourceReward.resourceData.GetLocalizedView(session, showCountIfSingle: false);
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
        return new[] { resourceReward };
    }
}
