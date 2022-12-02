using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceRangeReward : RewardBase
    {
        [JsonProperty]
        public ResourceType resourceType = ResourceType.Gold;
        [JsonProperty]
        public int amountMin;
        [JsonProperty]
        public int amountMax;

        public override Task<string> AddReward(GameSession session)
        {
            var amount = new Random().Next(amountMin, amountMax + 1);
            session.player.resources.ForceAdd(resourceType, amount);
            var result = resourceType.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }
    }
}
