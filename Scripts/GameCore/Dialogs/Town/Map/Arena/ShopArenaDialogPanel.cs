using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
public enum ShopArenaCategory : byte { None = 0, Main, Exchange, Temporary }

public sealed class ShopArenaDialogPanel : DialogPanelBase
{
    private ShopArenaCategory _currentCategory;

    private PlayerResources playerResources => session.player.resources;
    private ArenaShopSettings arenaShopSettings => gameDataHolder.arenaShopSettings[session.player.buildings.GetBuildingLevel(BuildingId.TownHall)];

    public ShopArenaDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        await ShowCategories().FastAwait();
    }

    private async Task ShowCategories()
    {
        ClearButtons();
        RegisterCategory(ShopArenaCategory.Temporary);
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
            .Append(Localization.Get(session, "dialog_arena_shop_select_category_header"));

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
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
                // TODO
                return;
            case ShopArenaCategory.Main:
                await ShowMainCategory().FastAwait();
                return;
            case ShopArenaCategory.Exchange:
                await ShowExchangeCategory().FastAwait();
                return;
        }
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
            onSuccess: () => new ShopArenaDialog(session).StartWithCategory(_currentCategory),
            onFail: () => OnPurchaseFail(shopItem))
            .FastAwait();
    }

    private async Task OnPurchaseFail(ShopItemBase shopItem)
    {
        var sb = new StringBuilder()
            .AppendLine(shopItem.GetTitle(session))
            .AppendLine()
            .AppendLine(Localization.Get(session, "resource_not_enough"));

        ClearButtons();
        RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowCategory(_currentCategory));

        await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

}
