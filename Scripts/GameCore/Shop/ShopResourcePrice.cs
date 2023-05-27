using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop;
[JsonObject]
public class ShopResourcePrice : ShopPriceBase
{
    public override ShopPriceType priceType => ShopPriceType.ResourcePrice;

    public ResourceId resourceId { get; set; }
    public int amount { get; set; }

    [JsonIgnore]
    public ResourceData resourceData { get; private set; }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        resourceData = new ResourceData(resourceId, amount);
    }

    public override string GetCompactView()
    {
        return resourceData.GetCompactView(shortView: false);
    }

    public override string GetPriceView(GameSession session)
    {
        return resourceData.GetPriceView(session);
    }

    public override Task<bool> TryPurchase(GameSession session)
    {
        return Task.FromResult(session.player.resources.TryPurchase(resourceData));
    }
}
