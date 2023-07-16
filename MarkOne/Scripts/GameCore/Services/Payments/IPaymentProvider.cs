using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Services.Payments;
internal interface IPaymentProvider
{
    PaymentProviderType providerType { get; }
    Task<CreatedPaymentInfo?> CreatePayment(GameSession session, PaymentData paymentData);
}
