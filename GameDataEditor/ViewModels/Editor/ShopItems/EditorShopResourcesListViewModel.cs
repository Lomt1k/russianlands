using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.Rewards;
using GameDataEditor.ViewModels.UserControls;
using GameDataEditor.Views.Editor.Rewards;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;
using System.Threading.Tasks;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class EditorShopResourcesListViewModel : EditorListViewModel<ResourceReward>
{
    protected override Task<ResourceReward?> CreateNewListItem()
    {
        var reward = new ResourceReward(ResourceId.Gold, 0);
        return Task.FromResult(reward);
    }

    protected override UserControl CreateViewForItem(ResourceReward reward)
    {
        return new EditorResourceRewardView() { DataContext = new EditorResourceRewardViewModel(reward) };
    }
}
