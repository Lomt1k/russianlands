using GameDataEditor.Views.Editor.ShopItems;
using GameDataEditor.Views.UserControls;
using MarkOne.Scripts.GameCore.Shop;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class ShopResourceBoxItemViewModel
{
    public ShopResourceBoxItem shopResourceBoxItem { get; }
    public EditorListView rewardsView { get; }
    public ShopPriceView priceView { get; }

    public ShopResourceBoxItemViewModel(ShopResourceBoxItem _shopResourceBoxItem)
    {
        shopResourceBoxItem = _shopResourceBoxItem;
        var listViewModel = new EditorShopResourcesListViewModel();
        listViewModel.SetModel(shopResourceBoxItem.rewards);
        rewardsView = new EditorListView() { DataContext = listViewModel };
        priceView = new ShopPriceView() { DataContext = new ShopPriceViewModel(shopResourceBoxItem) };
    }
}
