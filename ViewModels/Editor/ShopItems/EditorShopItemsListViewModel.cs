﻿using Avalonia.Controls;
using MarkOne.Models.RegularDialogs;
using MarkOne.Scripts.GameCore.Shop;
using MarkOne.ViewModels.UserControls;
using MarkOne.Views.Editor.ShopItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkOne.ViewModels.Editor.ShopItems;
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
        };
    }
}
