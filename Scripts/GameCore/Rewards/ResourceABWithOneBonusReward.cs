using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.Sessions;
using System.Text;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceABWithOneBonusReward : RewardBase
    {
        [JsonProperty]
        public ResourceType resourceTypeA = ResourceType.Gold;
        [JsonProperty]
        public int amountA;

        [JsonProperty]
        public ResourceType resourceTypeB = ResourceType.Wood;
        [JsonProperty]
        public int amountB;

        [JsonProperty]
        public int bonusA_min;
        [JsonProperty]
        public int bonusA_max;
        [JsonProperty]
        public int bonusB_min;
        [JsonProperty]
        public int bonusB_max;


        public override Task<string> AddReward(GameSession session)
        {
            var isA = new Random().Next(2) == 0;
            return isA ? AddRewardWithBonusA(session) : AddRewardWithBonusB(session);
        }

        private Task<string> AddRewardWithBonusA(GameSession session)
        {
            var amountWithBonus = amountA + new Random().Next(bonusA_min, bonusA_max + 1);
            session.player.resources.ForceAdd(resourceTypeA, amountWithBonus);
            session.player.resources.ForceAdd(resourceTypeB, amountB);

            var sb = new StringBuilder();
            sb.AppendLine(resourceTypeA.GetLocalizedView(session, amountWithBonus));
            sb.Append(resourceTypeB.GetLocalizedView(session, amountB));
            
            return Task.FromResult(sb.ToString());
        }

        private Task<string> AddRewardWithBonusB(GameSession session)
        {
            session.player.resources.ForceAdd(resourceTypeA, amountA);
            var amountWithBonus = amountB + new Random().Next(bonusB_min, bonusB_max + 1);
            session.player.resources.ForceAdd(resourceTypeB, amountWithBonus);

            var sb = new StringBuilder();
            sb.AppendLine(resourceTypeA.GetLocalizedView(session, amountA));
            sb.Append(resourceTypeB.GetLocalizedView(session, amountWithBonus));

            return Task.FromResult(sb.ToString());
        }

    }
}
