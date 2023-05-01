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
        public ResourceId resourceId { get; set; } = ResourceId.Gold;
        public int amountMin { get; set; }
        public int amountMax { get; set; }

        public override Task<string?> AddReward(GameSession session)
        {
            var amount = new Random().Next(amountMin, amountMax + 1);
            session.player.resources.ForceAdd(resourceId, amount);
            var result = resourceId.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }
    }
}
