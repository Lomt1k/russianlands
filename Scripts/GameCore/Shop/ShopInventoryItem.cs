using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopInventoryItem : ShopItemBase
{
    public ItemWithCodeReward itemWithCodeReward { get; set; } = new();

    public override string GetTitle(GameSession session)
    {
        return itemWithCodeReward.itemTemplate.GetFullName(session);
    }

    public override string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(itemWithCodeReward.itemTemplate.GetView(session));

        if (price != null)
        {
            sb.AppendLine(price.GetPlayerResourcesView(session));
        }

        return sb.ToString();
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return new[] { itemWithCodeReward };
    }
}
