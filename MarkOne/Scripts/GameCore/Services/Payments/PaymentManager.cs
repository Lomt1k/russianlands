using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;
using System;
using System.Threading.Tasks;
using static MarkOne.Scripts.Bot.BotConfig;

namespace MarkOne.Scripts.GameCore.Services.Payments;
public class PaymentManager : Service
{
    private readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private bool _isEnabled;
    private PaymentsSettings _settings;
    private IPaymentProvider? _paymentProvider;

    public override Task OnBotStarted()
    {
        _settings = BotController.config.paymentsSettings;
        _isEnabled = _settings.isEnabled;
        if (_isEnabled)
        {
            switch (_settings.paymentProvider)
            {
                case PaymentProviderType.LAVA_RU:
                    _paymentProvider = new LavaPaymentProvider(_settings);
                    break;
            }
        }
        return Task.CompletedTask;
    }

    public async Task<PaymentData?> CreatePayment(GameSession session, ShopItemBase shopItem, double rubblePrice)
    {
        if (!_isEnabled || _paymentProvider is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var paymentData = new PaymentData
        {
            telegramId = session.actualUser.Id,
            providerType = _paymentProvider.providerType,
            vendorCode = shopItem.vendorCode,
            status = PaymentStatus.NotPaid,
            rubbles = rubblePrice,
            creationDate = now,
            expireDate = now.AddMinutes(_settings.expireTimeInMinutes)
        };
        var db = BotController.dataBase.db;
        await db.InsertAsync(paymentData).FastAwait();

        var success = await _paymentProvider.CreatePayment(session, shopItem, paymentData).FastAwait();
        return success ? paymentData : null;
    }
}
