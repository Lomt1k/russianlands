using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;
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

    public override string GetCompactPriceView()
    {
        return resourceData.GetCompactView(shortView: false);
    }

    public override string GetPlayerResourcesView(GameSession session)
    {
        var playerResourceData = session.player.resources.GetResourceData(resourceId);

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "resource_header_ours"))
            .Append(playerResourceData.GetLocalizedView(session));

        return sb.ToString();
    }

    public override async Task<bool> TryPurchase(GameSession session, Func<string, Task> onPurchaseError)
    {
        var success = session.player.resources.TryPurchase(resourceData, out var notEnoughResource);
        if (!success)
        {
            var purchaseError = new StringBuilder()
                .AppendLine(Localization.Get(session, "resource_not_enough"))
                .AppendLine()
                .AppendLine(Localization.Get(session, "resource_header_resources"))
                .AppendLine(notEnoughResource.GetLocalizedView(session))
                .ToString();
            await onPurchaseError(purchaseError).FastAwait();
        }
        return success;
    }
}
