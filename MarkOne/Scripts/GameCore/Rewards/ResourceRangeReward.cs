using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public class ResourceRangeReward : ResourceRewardBase
{
    public ResourceId resourceId { get; set; } = ResourceId.Gold;
    public int amountMin { get; set; }
    public int amountMax { get; set; }

    public ResourceRangeReward(ResourceId _resourceId, int _amountMin, int _amountMax)
    {
        resourceId = _resourceId;
        amountMin = _amountMin;
        amountMax = _amountMax;
    }

    public override Task<string?> AddReward(GameSession session)
    {
        var amount = new Random().Next(amountMin, amountMax + 1);
        var resourceData = new ResourceData(resourceId, amount);
        session.player.resources.ForceAdd(resourceData);
        var text = resourceData.GetLocalizedView(session);
        return Task.FromResult(text);
    }

    public override Task<string?> AddRewardWithAddPossiblePremiumRewardToList(GameSession session, List<ResourceReward> possiblePremiumRewards)
    {
        var amount = new Random().Next(amountMin, amountMax + 1);
        var premiumAmount = amount * 15 / 100;
        if (premiumAmount > 0)
        {
            possiblePremiumRewards.Add(new ResourceReward(resourceId, premiumAmount));
        }

        var resourceData = new ResourceData(resourceId, amount);
        session.player.resources.ForceAdd(resourceData);
        var text = resourceData.GetLocalizedView(session);
        return Task.FromResult(text);
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        var minResourceData = new ResourceData(resourceId, amountMin);
        return minResourceData.GetLocalizedView(session) + $" - {amountMax.View()}";
    }

}
