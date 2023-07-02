using Avalonia.Controls;
using MarkOne.Scripts.GameCore.Rewards;
using GameDataEditor.ViewModels.Editor.Rewards;
using GameDataEditor.ViewModels.UserControls;
using GameDataEditor.Views.Editor.Rewards;
using System.Threading.Tasks;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
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
