using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Services.Payments;
public class PaymentInfo
{
    public PaymentData data { get; }
    public string url { get; }

    public PaymentInfo(PaymentData _paymentData, string? _url)
    {
        data = _paymentData;
        url = _url;
    }
}
