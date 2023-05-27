using JsonKnownTypes;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<ShopPriceBase>))]
public abstract class ShopPriceBase
{
    public abstract ShopPriceType priceType { get; }

    public abstract string GetCompactView();
    public abstract string GetPriceView(GameSession session);
    public abstract Task<bool> TryPurchase(GameSession session);
}
