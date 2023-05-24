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
    public List<RewardBase> rewards { get; set; } = new();

    protected override string GetTitle(GameSession session)
    {
        return Localization.Get(session, titleLocalizationKey);
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
