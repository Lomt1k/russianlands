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
        public ResourceType resourceTypeA { get; set; } = ResourceType.Gold;
        public int amountA { get; set; }

        public ResourceType resourceTypeB { get; set; } = ResourceType.Wood;
        public int amountB { get; set; }

        public int bonusA_min { get; set; }
        public int bonusA_max { get; set; }
        public int bonusB_min { get; set; }
        public int bonusB_max { get; set; }


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
