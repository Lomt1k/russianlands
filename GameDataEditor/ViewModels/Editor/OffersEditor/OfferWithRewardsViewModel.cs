using GameDataEditor.ViewModels.Editor.Rewards;
using GameDataEditor.Views.UserControls;
using MarkOne.Scripts.GameCore.Shop.Offers;

namespace GameDataEditor.ViewModels.Editor.OffersEditor;
public class OfferWithRewardsViewModel : OfferViewModel
{
    public EditorListView rewardView { get; set; }

    public OfferWithRewardsViewModel(OfferWithRewardsData offerData) : base(offerData)
    {
        var viewModel = new EditorRewardsListViewModel();
        viewModel.SetModel(offerData.rewards);
        rewardView = new EditorListView() { DataContext = viewModel };
    }
}
