using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using System.Runtime.Serialization;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public class ResourceReward : ResourceRewardBase
{
    public ResourceId resourceId { get; set; } = ResourceId.Gold;
    public int amount { get; set; }

    [JsonIgnore]
    public ResourceData resourceData { get; private set; }

    public ResourceReward(ResourceId _resourceId, int _amount)
    {
        resourceId = _resourceId;
        amount = _amount;
        resourceData = new ResourceData(resourceId, amount);
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        resourceData = new ResourceData(resourceId, amount);
    }

    public override Task<string> AddReward(GameSession session)
    {
        var addedResource = session.player.resources.Add(resourceData);
        var result = resourceData.GetLocalizedView(session)
            + (addedResource.amount < resourceData.amount ? Localization.Get(session, "resource_full_storage_prefix") : string.Empty);
        return Task.FromResult(result);
    }

    public override Task<string?> AddRewardWithAddPossiblePremiumRewardToList(GameSession session, List<ResourceReward> possiblePremiumRewards)
    {
        var premiumAmount = amount * 15 / 100;
        if (premiumAmount > 0)
        {
            possiblePremiumRewards.Add(new ResourceReward(resourceId, premiumAmount));
        }
        return AddReward(session);
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        return resourceData.resourceId.IsCraftResource()
            ? resourceData.GetLocalizedView(session, showCountIfSingle: true)
            : resourceData.GetLocalizedView(session, showCountIfSingle: false);
    }

}
