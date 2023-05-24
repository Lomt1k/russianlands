using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopRandomInventoryItem : ShopItemBase
{
    // TODO: Случайный предмет конкретного типа и редкости (редкий меч, эпический посох и тд)
    // TODO: Нужно завести новый тип реварда для этого
    protected override string GetTitle(GameSession session)
    {
        throw new NotImplementedException();
    }

    protected override string GetPossibleRewardsView(GameSession session)
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        throw new NotImplementedException();
    }
}
