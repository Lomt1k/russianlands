using MarkOne.Scripts.GameCore.Shop;
using GameDataEditor.Views.Editor.Rewards;
using GameDataEditor.Views.Editor.ShopItems;
using GameDataEditor.ViewModels.Editor.Rewards;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class ShopResourceItemViewModel : ViewModelBase
{
    public ShopResourceItem shopResourceItem { get; }
    public EditorResourceRewardView rewardView { get; }
    public ShopPriceView priceView { get; }

    public ShopResourceItemViewModel(ShopResourceItem _shopResourceItem)
    {
        shopResourceItem = _shopResourceItem;
        rewardView = new EditorResourceRewardView() { DataContext = new EditorResourceRewardViewModel(shopResourceItem.resourceReward) };
        priceView = new ShopPriceView() { DataContext = new ShopPriceViewModel(shopResourceItem) };
    }
}
