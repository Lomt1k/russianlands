using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopLootboxItem : ShopItemBase
{
    public string titleLocalizationKey { get; set; } = string.Empty;
    public List<RandomItemReward> rewards { get; set; } = new();

    protected override string GetTitle(GameSession session)
    {
        return GetEmojiForTitle() + Localization.Get(session, titleLocalizationKey);
    }

    private Emoji GetEmojiForTitle()
    {
        if (rewards.Count > 1)
        {
            return Emojis.ItemUnknown;
        }
        var singleReward = rewards[0];
        return singleReward.itemType == ItemType.Any ? Emojis.ItemUnknown : singleReward.itemType.GetEmoji();
    }

    protected override string GetPossibleRewardsView(GameSession session)
    {
        return rewards.GetPossibleRewardsView(session);
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return rewards;
    }
}
