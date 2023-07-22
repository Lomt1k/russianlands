using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Items.Generators;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
public enum ShopArenaCategory : byte { None = 0, Main, Exchange, Temporary }

public sealed class ShopArenaDialogPanel : DialogPanelBase
{
    private ShopArenaCategory _currentCategory;
    private List<ShopItemBase> _temporaryShopItems = new();
    private bool _isTemporaryItemsInitialized;
    private bool _isNeedCreateNewTemporaryItems;

    private PlayerResources playerResources => session.player.resources;
    private byte townhall => session.player.buildings.GetBuildingLevel(BuildingId.TownHall);
    private ArenaShopSettings arenaShopSettings => gameDataHolder.arenaShopSettings[townhall];

    public ShopArenaDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        await ShowCategories().FastAwait();
    }

    private async Task ShowCategories()
    {
        if (CanCollectFreeChips(out var freeChipsResourceData, out var timeToCollectChips))
        {
            await GiveFreeChips(freeChipsResourceData).FastAwait();
            return;
        }

        ClearButtons();
        if (arenaShopSettings.tempItemPatterns.Count > 0)
        {
            RegisterCategory(ShopArenaCategory.Temporary);
        }
        RegisterCategory(ShopArenaCategory.Main);
        RegisterCategory(ShopArenaCategory.Exchange);

        var resourcesToShow = new[]
        {
            playerResources.GetResourceData(ResourceId.ArenaChip),
            playerResources.GetResourceData(ResourceId.ArenaTicket),
        };        

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "resource_header_ours"))
            .AppendLine(resourcesToShow.GetCompactView(shortView: false))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_arena_shop_comeback_later_for_chips"))
            .AppendLine(timeToCollectChips.GetView(session))
            .AppendLine()
            .Append(Localization.Get(session, "dialog_arena_shop_select_category_header"));

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private bool CanCollectFreeChips(out ResourceData resourceData, out TimeSpan timeToCollect)
    {
        resourceData = arenaShopSettings.freeChipsResourceData;
        var cooldown = arenaShopSettings.freeChipsDelayInSeconds;
        var nextCollectTime = session.profile.data.lastCollectArenaChipsTime.AddSeconds(cooldown);
        var now = DateTime.UtcNow;
        timeToCollect = nextCollectTime - now;
        return now >= nextCollectTime;
    }

    private async Task GiveFreeChips(ResourceData resourceData)
    {
        Program.logger.Info($"User {session.actualUser} get free daily chips from arena shop");
        session.player.resources.Add(resourceData);
        session.profile.data.lastCollectArenaChipsTime = DateTime.UtcNow;

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "dialog_arena_shop_collected_free_chips"))
            .AppendLine()
            .AppendLine(Localization.Get(session, "battle_result_header_rewards"))
            .Append(resourceData.GetLocalizedView(session));

        ClearButtons();
        RegisterButton(Localization.Get(session, "menu_item_continue_button"), ShowCategories);
        await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

    private void RegisterCategory(ShopArenaCategory category)
    {
        RegisterButton(GetCategoryName(category, session), () => ShowCategory(category));
    }

    private string GetCategoryName(ShopArenaCategory category, GameSession session)
    {
        return category switch
        {
            ShopArenaCategory.Main => Localization.Get(session, "dialog_arena_shop_category_main"),
            ShopArenaCategory.Temporary => Localization.Get(session, "dialog_arena_shop_category_temporary"),
            ShopArenaCategory.Exchange => Localization.Get(session, "dialog_arena_shop_category_exchange") + Emojis.ResourceArenaTicket + " =>" + Emojis.ResourceArenaChip,
            _ => string.Empty
        };
    }

    public async Task ShowCategory(ShopArenaCategory category)
    {
        _currentCategory = category;
        switch (category)
        {
            case ShopArenaCategory.Temporary:
                await ShowTemporaryCategory().FastAwait();
                return;
            case ShopArenaCategory.Main:
                await ShowMainCategory().FastAwait();
                return;
            case ShopArenaCategory.Exchange:
                await ShowExchangeCategory().FastAwait();
                return;
        }
    }

    private async Task ShowTemporaryCategory()
    {
        ResetTemporaryItemsIfRequired(out var timeUntilNextRefresh);
        LoadTemporaryItemsIfNotLoaded();

        ClearButtons();
        foreach (var shopItem in _temporaryShopItems)
        {
            RegisterButton(shopItem.GetNameForList(session), () => ShowItem(shopItem));
        }

        var sb = new StringBuilder()
            .AppendLine(GetCategoryName(_currentCategory, session))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_arena_shop_refresh_temp_items_header"))
            .AppendLine(timeUntilNextRefresh.GetView(session));

        RegisterBackButton(Localization.Get(session, "dialog_arena_shop_button") + Emojis.ElementScales, ShowCategories);
        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowMainCategory()
    {
        ClearButtons();
        foreach (var shopItem in arenaShopSettings.mainCategoryItems)
        {
            RegisterButton(shopItem.GetNameForList(session), () => ShowItem(shopItem));
        }
        RegisterBackButton(Localization.Get(session, "dialog_arena_shop_button") + Emojis.ElementScales, ShowCategories);
        await SendPanelMessage(GetCategoryName(_currentCategory, session), GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowExchangeCategory()
    {
        ClearButtons();
        foreach (var shopItem in arenaShopSettings.exchangeCategoryItems)
        {
            RegisterButton(shopItem.GetNameForList(session), () => ShowItem(shopItem));
        }
        RegisterBackButton(Localization.Get(session, "dialog_arena_shop_button") + Emojis.ElementScales, ShowCategories);
        await SendPanelMessage(GetCategoryName(_currentCategory, session), GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowItem(ShopItemBase shopItem)
    {
        ClearButtons();
        RegisterButton(shopItem.GetPriceButtonView(session), () => TryPurchaseItem(shopItem));
        RegisterBackButton(() => ShowCategory(_currentCategory));
        RegisterDoubleBackButton(Localization.Get(session, "dialog_arena_shop_button") + Emojis.ElementScales, ShowCategories);

        var text = shopItem.GetMessageText(session);
        await SendPanelMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task TryPurchaseItem(ShopItemBase shopItem)
    {
        await shopItem.TryPurchase(session,
            onSuccess: () => OnSuccessPurchase(shopItem),
            onPurchaseError: (error) => OnPurchaseFail(error, shopItem))
            .FastAwait();
    }

    private async Task OnSuccessPurchase(ShopItemBase shopItem)
    {
        if (_currentCategory == ShopArenaCategory.Temporary)
        {
            RemoveTemporaryItem(shopItem);
        }
        await new ShopArenaDialog(session).StartWithCategory(_currentCategory).FastAwait();
    }

    private async Task OnPurchaseFail(string errorText, ShopItemBase shopItem)
    {
        var sb = new StringBuilder()
            .AppendLine(shopItem.GetTitle(session))
            .AppendLine()
            .AppendLine(errorText);

        ClearButtons();
        RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowCategory(_currentCategory));

        await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

    #region Temp Items

    private void ResetTemporaryItemsIfRequired(out TimeSpan timeUntilNextReset)
    {
        var now = DateTime.UtcNow;
        timeUntilNextReset = TimeSpan.FromSeconds(arenaShopSettings.tempItemsRefreshDelayInSeconds);
        var nextRefreshTime = session.profile.data.lastArenaItemsUpdateTime.Add(timeUntilNextReset);
        if (nextRefreshTime > now)
        {
            timeUntilNextReset = nextRefreshTime - now;
            return;
        }

        ResetTemporaryItems();
    }

    private void ResetTemporaryItems()
    {
        var profileData = session.profile.data;
        profileData.lastArenaItemsUpdateTime = DateTime.UtcNow;
        profileData.arenaItemId_0 = null;
        profileData.arenaItemId_1 = null;
        profileData.arenaItemId_2 = null;
        profileData.arenaItemId_3 = null;
        profileData.arenaItemId_4 = null;
        _temporaryShopItems.Clear();
        _isTemporaryItemsInitialized = false;
        _isNeedCreateNewTemporaryItems = true;
    }

    private void LoadTemporaryItemsIfNotLoaded()
    {
        if (_isTemporaryItemsInitialized)
        {
            return;
        }

        var pattensArray = GetItemPatterns().ToArray();
        var profileData = session.profile.data;

        // item 0
        if (pattensArray.Length > 0)
        {
            var pattern = pattensArray[0];
            var chipPrice = new ResourceData(ResourceId.ArenaChip, pattern.chipsPrice);
            if (profileData.arenaItemId_0 is not null)
            {
                _temporaryShopItems.Add(new ShopInventoryItem(profileData.arenaItemId_0, chipPrice));
            }
            else if (_isNeedCreateNewTemporaryItems)
            {
                var tempItem = ItemGenerationManager.GenerateItem(townhall, pattern.rarity);
                profileData.arenaItemId_0 = tempItem.id;
                _temporaryShopItems.Add(new ShopInventoryItem(tempItem, chipPrice));
            }
        }

        // item 1
        if (pattensArray.Length > 1)
        {
            var pattern = pattensArray[1];
            var chipPrice = new ResourceData(ResourceId.ArenaChip, pattern.chipsPrice);
            if (profileData.arenaItemId_1 is not null)
            {
                _temporaryShopItems.Add(new ShopInventoryItem(profileData.arenaItemId_1, chipPrice));
            }
            else if (_isNeedCreateNewTemporaryItems)
            {
                var tempItem = ItemGenerationManager.GenerateItem(townhall, pattern.rarity);
                profileData.arenaItemId_1 = tempItem.id;
                _temporaryShopItems.Add(new ShopInventoryItem(tempItem, chipPrice));
            }
        }

        // item 2
        if (pattensArray.Length > 2)
        {
            var pattern = pattensArray[2];
            var chipPrice = new ResourceData(ResourceId.ArenaChip, pattern.chipsPrice);
            if (profileData.arenaItemId_2 is not null)
            {
                _temporaryShopItems.Add(new ShopInventoryItem(profileData.arenaItemId_2, chipPrice));
            }
            else if (_isNeedCreateNewTemporaryItems)
            {
                var tempItem = ItemGenerationManager.GenerateItem(townhall, pattern.rarity);
                profileData.arenaItemId_2 = tempItem.id;
                _temporaryShopItems.Add(new ShopInventoryItem(tempItem, chipPrice));
            }
        }

        // item 3
        if (pattensArray.Length > 3)
        {
            var pattern = pattensArray[3];
            var chipPrice = new ResourceData(ResourceId.ArenaChip, pattern.chipsPrice);
            if (profileData.arenaItemId_3 is not null)
            {
                _temporaryShopItems.Add(new ShopInventoryItem(profileData.arenaItemId_3, chipPrice));
            }
            else if (_isNeedCreateNewTemporaryItems)
            {
                var tempItem = ItemGenerationManager.GenerateItem(townhall, pattern.rarity);
                profileData.arenaItemId_3 = tempItem.id;
                _temporaryShopItems.Add(new ShopInventoryItem(tempItem, chipPrice));
            }
        }

        // item 4
        if (pattensArray.Length > 4)
        {
            var pattern = pattensArray[4];
            var chipPrice = new ResourceData(ResourceId.ArenaChip, pattern.chipsPrice);
            if (profileData.arenaItemId_4 is not null)
            {
                _temporaryShopItems.Add(new ShopInventoryItem(profileData.arenaItemId_4, chipPrice));
            }
            else if (_isNeedCreateNewTemporaryItems)
            {
                var tempItem = ItemGenerationManager.GenerateItem(townhall, pattern.rarity);
                profileData.arenaItemId_4 = tempItem.id;
                _temporaryShopItems.Add(new ShopInventoryItem(tempItem, chipPrice));
            }
        }

        _isTemporaryItemsInitialized = true;
        _isNeedCreateNewTemporaryItems = false;
    }

    private IEnumerable<ArenaShopItemPattern> GetItemPatterns()
    {
        foreach (var pattern in arenaShopSettings.tempItemPatterns)
        {
            for (int i = 0; i < pattern.count; i++)
            {
                yield return pattern;
            }
        }
    }

    private void RemoveTemporaryItem(ShopItemBase shopItem)
    {
        _temporaryShopItems.Remove(shopItem);
        if (shopItem is ShopInventoryItem shopInventoryItem)
        {
            var itemCode = shopInventoryItem.itemWithCodeReward.itemCode;
            var profileData = session.profile.data;
            if (profileData.arenaItemId_0 != null && profileData.arenaItemId_0.Equals(itemCode))
            {
                profileData.arenaItemId_0 = null;
                return;
            }
            if (profileData.arenaItemId_1 != null && profileData.arenaItemId_1.Equals(itemCode))
            {
                profileData.arenaItemId_1 = null;
                return;
            }
            if (profileData.arenaItemId_2 != null && profileData.arenaItemId_2.Equals(itemCode))
            {
                profileData.arenaItemId_2 = null;
                return;
            }
            if (profileData.arenaItemId_3 != null && profileData.arenaItemId_3.Equals(itemCode))
            {
                profileData.arenaItemId_3 = null;
                return;
            }
            if (profileData.arenaItemId_4 != null && profileData.arenaItemId_4.Equals(itemCode))
            {
                profileData.arenaItemId_4 = null;
                return;
            }
        }
    }

    #endregion

}
