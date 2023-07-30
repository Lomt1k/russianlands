using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Offers;
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
        return Localization.Get(session, titleKey) + Emojis.StatPremium;
    }

    public override string GetDescription(GameSession session)
    {
        return Localization.Get(session, descriptionKey)
            + "\n\n"
            + Localization.Get(session, "premium_reward", premiumReward.timeSpan.GetView(session));
    }

    public override string GetBestBuyLabel(GameSession session)
    {
        return Localization.Get(session, bestBuyKey);
    }

    public override async Task StartOfferDialog(GameSession session, OfferItem offerItem, Func<Task> onClose)
    {
        await new PremiumOfferDialog(session, this, offerItem, onClose).Start().FastAwait();
    }
    
}
