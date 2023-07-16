using Avalonia.Controls;
using GameDataEditor.Models.RegularDialogs;
using GameDataEditor.ViewModels.UserControls;
using GameDataEditor.Views.Editor.ShopItems;
using MarkOne.Scripts.GameCore.Shop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class EditorShopItemsListViewModel : EditorListViewModel<ShopItemBase>
{
    protected override async Task<ShopItemBase?> CreateNewListItem()
    {
        ShopItemBase? result = null;
        await RegularDialogHelper.ShowItemSelectionDialog("Select item type:", new Dictionary<string, Action>()
        {
            {"Resource", () => result = new ShopResourceItem() },
            {"Inventory Item", () => result = new ShopInventoryItem() },
            {"Lootbox", () => result = new ShopLootboxItem() },
            {"Premium", () => result = new ShopPremiumItem() },
            {"Fruits", () => result = new ShopFruitsItem() }
        });
        return result;
    }

    protected override UserControl CreateViewForItem(ShopItemBase item)
    {
        return item switch
        {
            ShopResourceItem resourceItem => new ShopResourceItemView() { DataContext = new ShopResourceItemViewModel(resourceItem) },
            ShopInventoryItem inventoryItem => new ShopInventoryItemView() { DataContext = new ShopInventoryItemViewModel(inventoryItem) },
            ShopLootboxItem lootboxItem => new ShopLootboxItemView() { DataContext = new ShopLootboxItemViewModel(lootboxItem) },
            ShopPremiumItem premiumItem => new ShopPremiumItemView() { DataContext = new ShopPremiumItemViewModel(premiumItem) },
            ShopFruitsItem fruitsItem => new ShopFruitsItemView() { DataContext = new ShopFruitsItemViewModel(fruitsItem) },
        };
    }
}
