namespace MarkOne.Scripts.GameCore.Services.Payments;
public class CreatedPaymentInfo
{
    public string url { get; }
    public string signature { get; }

    public CreatedPaymentInfo(string _url, string _signature)
    {
        url = _url;
        signature = _signature;
    }
}
