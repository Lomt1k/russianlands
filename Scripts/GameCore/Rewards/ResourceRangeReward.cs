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
        public ResourceType resourceType { get; set; } = ResourceType.Gold;
        public int amountMin { get; set; }
        public int amountMax { get; set; }

        public override Task<string?> AddReward(GameSession session)
        {
            var amount = new Random().Next(amountMin, amountMax + 1);
            session.player.resources.ForceAdd(resourceType, amount);
            var result = resourceType.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }
    }
}
