using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;
using System.Text;

namespace MarkOne.Scripts.GameCore.Shop;
public class ShopFruitsItem : ShopItemBase
{
    public string titleLocalizationKey { get; set; } = string.Empty;
    public RandomFruitsReward randomFruitsReward { get; set; } = new();

    public override string GetTitle(GameSession session)
    {
        return Emojis.ItemUnknown + Localization.Get(session, titleLocalizationKey).Bold();
    }

    public override string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(GetTitle(session))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_shop_item_view_contains_header"))
            .Append(randomFruitsReward.GetPossibleRewardsView(session));

        if (price != null)
        {
            sb.AppendLine();
            sb.AppendLine(price.GetPlayerResourcesView(session));
        }

        return sb.ToString();
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return new[] { randomFruitsReward };
    }

}
