using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public class ResourceRangeReward : RewardBase
{
    public ResourceId resourceId { get; set; } = ResourceId.Gold;
    public int amountMin { get; set; }
    public int amountMax { get; set; }

    public override Task<string?> AddReward(GameSession session)
    {
        var amount = new Random().Next(amountMin, amountMax + 1);
        var resourceData = new ResourceData(resourceId, amount);
        session.player.resources.ForceAdd(resourceData);
        var text = resourceData.GetLocalizedView(session);
        return Task.FromResult(text);
    }
}
