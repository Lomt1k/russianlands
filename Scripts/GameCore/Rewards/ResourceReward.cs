using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public class ResourceReward : RewardBase
{
    public ResourceId resourceId { get; set; } = ResourceId.Gold;
    public int amount { get; set; }

    public override Task<string> AddReward(GameSession session)
    {
        var resourceData = new ResourceData(resourceId, amount);
        session.player.resources.ForceAdd(resourceData);
        var result = resourceData.GetLocalizedView(session);
        return Task.FromResult(result);
    }
}
