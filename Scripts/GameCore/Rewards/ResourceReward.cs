using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceReward : RewardBase
    {
        public ResourceType resourceType { get; set; } = ResourceType.Gold;
        public int amount { get; set; }

        public override Task<string> AddReward(GameSession session)
        {
            session.player.resources.ForceAdd(resourceType, amount);
            var result = resourceType.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }
    }
}
