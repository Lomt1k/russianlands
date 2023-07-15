using Newtonsoft.Json;

namespace MarkOne.Scripts.GameCore.Http.Payments;
[JsonObject]
public class LavaPaymentInfo
{
    public string invoice_id { get; set; } = string.Empty;
    public string order_id { get; set; } = string.Empty;

    public string status { get; set; } = string.Empty;
    public string pay_time { get; set; } = string.Empty;
    public double amount { get; set; }
    public string? custom_fields { get; set; }
    public double credited { get; set; }

    public bool IsSuccess()
    {
        return status == "success";
    }
}
