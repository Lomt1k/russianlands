using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceReward : RewardBase
    {
        [JsonProperty]
        public ResourceType resourceType = ResourceType.Gold;
        [JsonProperty]
        public int amount;

        public override Task<string> AddReward(GameSession session)
        {
            session.player.resources.ForceAdd(resourceType, amount);
            var result = resourceType.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }
    }
}
