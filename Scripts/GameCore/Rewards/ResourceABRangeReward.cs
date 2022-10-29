using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceABRangeReward : RewardBase
    {
        [JsonProperty]
        public ResourceType resourceTypeA = ResourceType.Gold;
        [JsonProperty]
        public int amountMinA;
        [JsonProperty]
        public int amountMaxA;

        [JsonProperty]
        public ResourceType resourceTypeB = ResourceType.Wood;
        [JsonProperty]
        public int amountMinB;
        [JsonProperty]
        public int amountMaxB;

        public override Task<string> AddReward(GameSession session)
        {
            var isA = new Random().Next(2) == 0;
            return isA ? AddRewardA(session) : AddRewardB(session);
        }

        private Task<string> AddRewardA(GameSession session)
        {
            var amount = new Random().Next(amountMinA, amountMaxA + 1);
            session.player.resources.ForceAdd(resourceTypeA, amount);
            var result = resourceTypeA.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }

        private Task<string> AddRewardB(GameSession session)
        {
            var amount = new Random().Next(amountMinB, amountMaxB + 1);
            session.player.resources.ForceAdd(resourceTypeB, amount);
            var result = resourceTypeB.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }

    }
}
