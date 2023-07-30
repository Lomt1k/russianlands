using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;
using System.Linq;

namespace MarkOne.Scripts.GameCore.Shop.Offers;
public class ShopOfferItem : ShopItemBase
{
    private RewardBase[] _rewards;

    public ShopOfferItem(OfferData offerData, IEnumerable<RewardBase> rewards)
    {
        _rewards = rewards.ToArray();
        vendorCode = offerData.vendorCode;
        price = new ShopCurrentcyPrice { russianRubles = offerData.priceRubles };
    }

    public override string GetMessageText(GameSession session)
    {
        // ignored
        return string.Empty;
    }

    public override string GetTitle(GameSession session)
    {
        // ignored
        return string.Empty;
    }

    protected override IEnumerable<RewardBase> GetRewards()
    {
        return _rewards;
    }
}
