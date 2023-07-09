using GameDataEditor.ViewModels.Editor.Rewards;
using GameDataEditor.Views.Editor.Rewards;
using GameDataEditor.Views.Editor.ShopItems;
using MarkOne.Scripts.GameCore.Shop;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class ShopFruitsItemViewModel : ViewModelBase
{
    public ShopFruitsItem shopFruitsItem { get; }
    public EditorRandomFruitsRewardView rewardView { get; }
    public ShopPriceView priceView { get; }

    public ShopFruitsItemViewModel(ShopFruitsItem _shopFruitsItem)
    {
        shopFruitsItem = _shopFruitsItem;
        rewardView = new EditorRandomFruitsRewardView() { DataContext = new EditorRandomFruitsRewardViewModel(shopFruitsItem.randomFruitsReward) };
        priceView = new ShopPriceView() { DataContext = new ShopPriceViewModel(shopFruitsItem) };
    }
}
