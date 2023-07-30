using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop.Offers;
public class PremiumOfferData : OfferData
{
    public PremiumReward premiumReward { get; set; } = new();

    public PremiumOfferData(int _id) : base(_id)
    {
    }

    public override string GetTitle(GameSession session)
    {
        return Localization.Get(session, titleKey);
    }

    public override Task StartOfferDialog(GameSession session)
    {
        throw new NotImplementedException();
    }
}
