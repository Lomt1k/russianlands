using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace MarkOne.Scripts.GameCore.Services.Payments;
[JsonConverter(typeof(StringEnumConverter))]
public enum PaymentProviderType
{
    None = 0,
    LAVA_RU
}
