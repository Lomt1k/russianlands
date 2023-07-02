using MarkOne.Scripts.GameCore.Shop;
using MarkOne.Views.Editor.Rewards;
using MarkOne.Views.Editor.ShopItems;

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
