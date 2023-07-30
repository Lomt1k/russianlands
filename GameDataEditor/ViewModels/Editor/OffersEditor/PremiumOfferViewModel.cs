using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.Rewards;
using GameDataEditor.Views.Editor.Rewards;
using MarkOne.Scripts.GameCore.Shop.Offers;

namespace GameDataEditor.ViewModels.Editor.OffersEditor;
public class PremiumOfferViewModel : OfferViewModel
{
    public UserControl rewardView { get; set; }

    public PremiumOfferViewModel(PremiumOfferData offerData) : base(offerData)
    {
        rewardView = new EditorPremiumRewardView()
        {
            DataContext = new EditorPremiumRewardViewModel(offerData.premiumReward)
        };
    }
}
