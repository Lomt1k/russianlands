using MarkOne.Scripts.GameCore.Dialogs.Offers;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop.Offers;
public class OfferWithRewardsData : OfferData
{
    public List<RewardBase> rewards = new();

    public OfferWithRewardsData(int _id) : base(_id)
    {
    }

    public override string GetTitle(GameSession session)
    {
        return Localization.Get(session, titleKey);
    }

    public override string GetDescription(GameSession session)
    {
        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, descriptionKey))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_shop_item_view_contains_header"));

        foreach (var reward in rewards)
        {
            sb.AppendLine(reward.GetPossibleRewardsView(session));
        }

        return sb.ToString();
    }

    public override string GetBestBuyLabel(GameSession session)
    {
        return Localization.Get(session, bestBuyKey);
    }

    public override async Task StartOfferDialog(GameSession session, OfferItem offerItem, Func<Task> onClose)
    {
        await new OfferWithRewardsDialog(session, this, offerItem, onClose).Start().FastAwait();
    }

    public override ShopItemBase GenerateShopItem()
    {
        return new ShopOfferItem(this, rewards);
    }
}
