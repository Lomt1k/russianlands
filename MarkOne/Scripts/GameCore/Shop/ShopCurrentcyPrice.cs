using MarkOne.Scripts.Bot;
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
    private readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();
    private static readonly SessionExceptionHandler sessionExceptionHandler = ServiceLocator.Get<SessionExceptionHandler>();

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
        // DEBUG LOGIC!
        var paymentInfo = await paymentManager.CreatePayment(session, shopItem, russianRubles).FastAwait();
        if (paymentInfo.url is not null)
        {
            await messageSender.SendTextDialog(session.chatId, "Надо оплатить кароч: \n" + paymentInfo.url).FastAwait();
        }
        else
        {
            await onPurchaseError("Че-то url битый \n" + paymentInfo.url).FastAwait();
        }
        await Task.Delay(5000);
        // TODO
        return false;
    }
}
