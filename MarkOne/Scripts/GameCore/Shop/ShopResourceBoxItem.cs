using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopResourceBoxItem : ShopItemBase
{
    public string titleLocalizationKey { get; set; } = string.Empty;
    public List<ResourceReward> rewards { get; set; } = new();

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
            .AppendLine(GetPossibleRewardsView(session));

        if (price != null)
        {
            sb.AppendLine();
            sb.AppendLine(price.GetPlayerResourcesView(session));
        }

        return sb.ToString();
    }

    private string GetPossibleRewardsView(GameSession session)
    {
        return rewards.GetPossibleRewardsView(session);
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return rewards;
    }
}
