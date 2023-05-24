using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    protected override Task GiveAndShowRewards(GameSession session, Func<Task> onContinue)
    {
        throw new NotImplementedException();
    }
}
