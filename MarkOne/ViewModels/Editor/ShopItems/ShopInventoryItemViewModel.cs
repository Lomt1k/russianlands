using MarkOne.Scripts.GameCore.Shop;
using MarkOne.ViewModels.Editor.Rewards;
using MarkOne.Views.Editor.Rewards;
using MarkOne.Views.Editor.ShopItems;

namespace MarkOne.ViewModels.Editor.ShopItems;
internal class ShopInventoryItemViewModel : ViewModelBase
{
    public ShopInventoryItem shopInventoryItem { get; }
    public EditorItemWithCodeRewardView rewardView { get; }
    public ShopPriceView priceView { get; }

    public ShopInventoryItemViewModel(ShopInventoryItem _shopInventoryItem)
    {
        shopInventoryItem = _shopInventoryItem;
        rewardView = new EditorItemWithCodeRewardView() { DataContext = new EditorItemWithCodeRewardViewModel(shopInventoryItem.itemWithCodeReward) };
        priceView = new ShopPriceView() { DataContext = new ShopPriceViewModel(shopInventoryItem) };
    }
}
