using Avalonia.Controls;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.ViewModels.Editor.Rewards;
using MarkOne.ViewModels.UserControls;
using MarkOne.Views.Editor.Rewards;
using System.Threading.Tasks;

namespace MarkOne.ViewModels.Editor.ShopItems;
internal class EditorShopRandomItemsListViewModel : EditorListViewModel<RandomItemReward>
{
    protected override Task<RandomItemReward?> CreateNewListItem()
    {
        var reward = new RandomItemReward();
        return Task.FromResult(reward);
    }

    protected override UserControl CreateViewForItem(RandomItemReward reward)
    {
        return new EditorRandomItemRewardView() { DataContext = new EditorRandomItemRewardViewModel(reward) };
    }
}
