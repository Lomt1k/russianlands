using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Payments;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopCurrentcyPrice : ShopPriceBase
{
    private readonly PaymentManager paymentManager = ServiceLocator.Get<PaymentManager>();

    public override ShopPriceType priceType => ShopPriceType.CurrencyPrice;

    public uint russianRubles { get; set; }

    public override string GetCompactPriceView()
    {
        return $"{russianRubles} ₽";
    }

    public override string GetPlayerResourcesView(GameSession session)
    {
        return string.Empty;
    }

    public override async Task<bool> TryPurchase(GameSession session, ShopItemBase shopItem, Func<string, Task> onPurchaseError)
    {
        var vendorCode = shopItem.vendorCode;
        var comment = shopItem.GetTitle(session).RemoveHtmlTags();
        var paymentData = await paymentManager.TryGetOrCreatePayment(session, russianRubles, vendorCode, comment).FastAwait();
        if (paymentData is not null && session.currentDialog is ShopDialog shopDialog)
        {
            await shopDialog.ShowPaymentMessage(paymentData, shopItem).FastAwait();
        }
        else
        {
            await onPurchaseError(Localization.Get(session, "dialog_shop_error_on_create_payment")).FastAwait();
        }        
        return false;
    }
}
