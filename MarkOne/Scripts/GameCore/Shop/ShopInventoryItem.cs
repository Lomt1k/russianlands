using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;
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

    [JsonConstructor]
    public ShopInventoryItem()
    {
    }

    public ShopInventoryItem(string itemCode, ResourceData? resourcePrice = null)
    {
        itemWithCodeReward = new ItemWithCodeReward(itemCode);
        if (resourcePrice.HasValue)
        {
            price = new ShopResourcePrice(resourcePrice.Value);
        }
    }

    public ShopInventoryItem(InventoryItem inventoryItem, ResourceData? resourcePrice = null)
    {
        itemWithCodeReward = new ItemWithCodeReward(inventoryItem);
        if (resourcePrice.HasValue)
        {
            price = new ShopResourcePrice(resourcePrice.Value);
        }
    }

    public override string GetTitle(GameSession session)
    {
        return itemWithCodeReward.itemTemplate.GetFullName(session).Bold();
    }

    public override string GetMessageText(GameSession session)
    {
        var itemForView = itemWithCodeReward.itemTemplate.Clone();
        itemForView.RecalculateDataWithPlayerSkills(session.player.skills);

        var sb = new StringBuilder()
            .AppendLine(itemForView.GetView(session));

        if (price != null)
        {
            sb.AppendLine();
            sb.AppendLine(price.GetPlayerResourcesView(session));
        }

        return sb.ToString();
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return new[] { itemWithCodeReward };
    }
}
