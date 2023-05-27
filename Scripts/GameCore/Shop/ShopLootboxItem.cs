using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.Generators;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopLootboxItem : ShopItemBase
{
    public string titleLocalizationKey { get; set; } = string.Empty;
    public List<RandomItemReward> rewards { get; set; } = new();
    public bool isSingleReward => rewards.Count == 1 && rewards[0].count == 1;

    public override string GetTitle(GameSession session)
    {
        return isSingleReward ? GetPossibleRewardsView(session) : GetEmojiForTitle() + Localization.Get(session, titleLocalizationKey);
    }

    public override string GetMessageText(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(GetTitle(session).Bold());

        if (!isSingleReward)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_shop_item_view_contains_header"));
            sb.AppendLine(GetPossibleRewardsView(session));
        }

        sb.AppendLine();
        var minTownhall = rewards.Min(x => x.townhallLevel);
        var maxTownhall = rewards.Max(x => x.townhallLevel);
        var minLevel = ItemGenerationHelper.CalculateRequiredLevel(minTownhall, 1);
        var maxLevel = ItemGenerationHelper.CalculateRequiredLevel(maxTownhall, 10);
        sb.AppendLine(Localization.Get(session, "item_view_possible_level_range_requirments", minLevel, maxLevel));

        if (price != null)
        {
            sb.AppendLine();
            sb.AppendLine(price.GetPlayerResourcesView(session));
        }

        return sb.ToString();
    }

    private Emoji GetEmojiForTitle()
    {
        if (!isSingleReward)
        {
            return Emojis.ItemUnknown;
        }
        var singleReward = rewards[0];
        return singleReward.itemType == ItemType.Any ? Emojis.ItemUnknown : singleReward.itemType.GetEmoji();
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
