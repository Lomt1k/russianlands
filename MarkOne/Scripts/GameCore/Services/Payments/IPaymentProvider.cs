using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Services.Payments;
internal interface IPaymentProvider
{
    PaymentProviderType providerType { get; }
    Task<bool> CreatePayment(GameSession session, ShopItemBase shopItem, PaymentData paymentData);
}
