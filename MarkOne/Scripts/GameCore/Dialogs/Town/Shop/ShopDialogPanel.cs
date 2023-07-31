using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;
using MarkOne.Scripts.GameCore.Shop.Offers;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Shop;

public enum ShopCategory : byte { None = 0, Offers, Premium, Main, Diamonds }

internal class ShopDialogPanel : DialogPanelBase
{
    public static ResourceData[] premiumDailyRewards = new[]
    {
        new ResourceData(ResourceId.ArenaTicket, 1),
        new ResourceData(ResourceId.Diamond, 10),
    };

    public static int maxPremiumDays = 40;

    private ShopCategory _currentCategory;

    private PlayerResources playerResources => session.player.resources;
    private byte townhall => session.player.buildings.GetBuildingLevel(BuildingId.TownHall);
    private ShopSettings shopSettings => gameDataHolder.shopSettings[townhall];

    public ShopDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        await ShowCategories().FastAwait();
    }

    private async Task ShowCategories()
    {
        ClearButtons();

        var hasActiveOffers = session.profile.dynamicData.offers.Any(x => x.IsActive());
        if (hasActiveOffers)
        {
            RegisterCategory(ShopCategory.Offers);
        }

        RegisterCategory(ShopCategory.Premium);
        if (shopSettings.lootboxCategoryItems.Count > 0)
        {
            RegisterCategory(ShopCategory.Main);
        }
        RegisterCategory(ShopCategory.Diamonds);

        var resourcesToShow = new[]
        {
            playerResources.GetResourceData(ResourceId.Diamond),
        };

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "resource_header_ours"))
            .AppendLine(resourcesToShow.GetCompactView(shortView: false))
            .AppendLine()
            .Append(Localization.Get(session, "dialog_shop_select_category_header"));

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private void RegisterCategory(ShopCategory category)
    {
        RegisterButton(GetCategoryName(category, session), () => ShowCategory(category));
    }

    private string GetCategoryName(ShopCategory category, GameSession session)
    {
        return category switch
        {
            ShopCategory.Offers => Emojis.ElementWarning + Localization.Get(session, "dialog_shop_category_offers"),
            ShopCategory.Premium => Emojis.StatPremium + Localization.Get(session, "dialog_shop_category_premium"),
            ShopCategory.Main => Localization.Get(session, "dialog_shop_category_treasures"),
            ShopCategory.Diamonds => Emojis.ResourceDiamond + Localization.Get(session, "dialog_shop_category_diamonds"),
            _ => string.Empty
        };
    }

    public async Task ShowCategory(ShopCategory category)
    {
        _currentCategory = category;
        switch (category)
        {
            case ShopCategory.Offers:
                await ShowOffersCategory().FastAwait();
                return;
            case ShopCategory.Premium:
                await ShowPremiumCategory().FastAwait();
                return;
            case ShopCategory.Main:
                await ShowMainCategory().FastAwait();
                return;
            case ShopCategory.Diamonds:
                await ShowDiamondsCategory().FastAwait();
                return;
        }
    }

    private async Task ShowOffersCategory()
    {
        var activeOffers = session.profile.dynamicData.offers
            .Where(x => x.IsActive())
            .OrderBy(x => x.GetTimeToEnd())
            .ToArray();

        ClearButtons();
        foreach (var offerItem in activeOffers)
        {
            var buttonText = offerItem.GetTitleForList(session);
            RegisterButton(buttonText, () => ShowOffer(offerItem));
        }
        RegisterBackButton(Localization.Get(session, "menu_item_shop") + Emojis.ElementScales, ShowCategories);
        await SendPanelMessage(GetCategoryName(_currentCategory, session), GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowOffer(OfferItem offerItem)
    {
        var offerData = offerItem.GetData();
        await offerData.StartOfferDialog(session, offerItem, () => new ShopDialog(session).StartWithCategory(_currentCategory));
    }

    private async Task ShowPremiumCategory()
    {
        ClearButtons();
        var timeUntilExpire = session.profile.data.endPremiumTime - DateTime.UtcNow;
        foreach (var shopItem in shopSettings.premiumCategoryItems)
        {
            if (timeUntilExpire.Days > maxPremiumDays)
            {
                RegisterButton(shopItem.GetNameForList(session), ShowPremiumLimitReachedMessage);
            }
            else
            {
                RegisterButton(shopItem.GetNameForList(session), () => ShowItem(shopItem));
            }
        }
        RegisterBackButton(Localization.Get(session, "menu_item_shop") + Emojis.ElementScales, ShowCategories);

        var sb = new StringBuilder()
            .AppendLine(GetCategoryName(_currentCategory, session))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_shop_premium_description", Emojis.ElementSmallBlack, Emojis.StatPremium,
                premiumDailyRewards[0].GetLocalizedView(session, showCountIfSingle: false),
                premiumDailyRewards[1].GetCompactView()));

        if (session.profile.data.IsPremiumActive())
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_shop_premium_end_time", timeUntilExpire.GetShortView(session)));
        }

        await SendPanelMessage(sb.ToString(), GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowPremiumLimitReachedMessage()
    {
        ClearButtons();
        RegisterBackButton(Localization.Get(session, "menu_item_shop") + Emojis.ElementScales, ShowCategories);
        var text = Localization.Get(session, "dialog_shop_premium_limit_reached", maxPremiumDays);
        await SendPanelMessage(text, GetOneLineKeyboard()).FastAwait();
    }

    private async Task ShowMainCategory()
    {
        ClearButtons();
        foreach (var shopItem in shopSettings.lootboxCategoryItems)
        {
            RegisterButton(shopItem.GetNameForList(session), () => ShowItem(shopItem));
        }
        RegisterBackButton(Localization.Get(session, "menu_item_shop") + Emojis.ElementScales, ShowCategories);
        await SendPanelMessage(GetCategoryName(_currentCategory, session), GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowDiamondsCategory()
    {
        ClearButtons();
        foreach (var shopItem in shopSettings.diamondsCategoryItems)
        {
            RegisterButton(shopItem.GetNameForList(session), () => ShowItem(shopItem));
        }
        RegisterBackButton(Localization.Get(session, "menu_item_shop") + Emojis.ElementScales, ShowCategories);

        var sb = new StringBuilder();
        sb.AppendLine(GetCategoryName(_currentCategory, session));
        if (!session.profile.data.isDoubleDiamondsBonusUsed)
        {
            sb.AppendLine();
            sb.AppendLine(Emojis.ElementWarningRed.ToString() + Localization.Get(session, "dialog_shop_double_diamonds_bonus"));
        }
        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowItem(ShopItemBase shopItem)
    {
        ClearButtons();
        RegisterButton(shopItem.GetPriceButtonView(session), () => TryPurchaseItem(shopItem));
        RegisterBackButton(() => ShowCategory(_currentCategory));
        RegisterDoubleBackButton(Localization.Get(session, "menu_item_shop") + Emojis.ElementScales, ShowCategories);

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
        await new ShopDialog(session).StartWithCategory(_currentCategory).FastAwait();
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

    public async Task ShowPaymentMessage(PaymentData paymentData, ShopItemBase shopItem)
    {
        var buttonText = Localization.Get(session, "dialog_shop_buy_link_button");
        var messageText = Localization.Get(session, "dialog_shop_before_purchase", shopItem.GetTitle(session), buttonText);
        var url = paymentData.url;

        ClearButtons();
        RegisterLinkButton(Emojis.ButtonPay + buttonText, url);
        RegisterBackButton(() => ShowCategory(_currentCategory));

        await SendPanelMessage(messageText, GetMultilineKeyboard()).FastAwait();
    }

}
