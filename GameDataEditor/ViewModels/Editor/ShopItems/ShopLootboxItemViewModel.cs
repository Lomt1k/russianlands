using MarkOne.Scripts.GameCore.Shop;
using MarkOne.Views.Editor.ShopItems;
using MarkOne.Views.UserControls;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class ShopLootboxItemViewModel
{
    public ShopLootboxItem shopLootboxItem { get; }
    public EditorListView rewardsView { get; }
    public ShopPriceView priceView { get; }

    public ShopLootboxItemViewModel(ShopLootboxItem _shopLootboxItem)
    {
        shopLootboxItem = _shopLootboxItem;
        var listViewModel = new EditorShopRandomItemsListViewModel();
        listViewModel.SetModel(shopLootboxItem.rewards);
        rewardsView = new EditorListView() { DataContext = listViewModel };
        priceView = new ShopPriceView() { DataContext = new ShopPriceViewModel(shopLootboxItem) };
    }
}
