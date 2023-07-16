using GameDataEditor.ViewModels.Editor.Rewards;
using GameDataEditor.Views.Editor.Rewards;
using GameDataEditor.Views.Editor.ShopItems;
using MarkOne.Scripts.GameCore.Shop;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class ShopPremiumItemViewModel : ViewModelBase
{
    public ShopPremiumItem shopPremiumItem { get; }
    public EditorPremiumRewardView rewardView { get; }
    public ShopPriceView priceView { get; }

    public ShopPremiumItemViewModel(ShopPremiumItem _shopPremiumItem)
    {
        shopPremiumItem = _shopPremiumItem;
        rewardView = new EditorPremiumRewardView() { DataContext = new EditorPremiumRewardViewModel(shopPremiumItem.premiumReward) };
        priceView = new ShopPriceView() { DataContext = new ShopPriceViewModel(shopPremiumItem) };
    }
}
