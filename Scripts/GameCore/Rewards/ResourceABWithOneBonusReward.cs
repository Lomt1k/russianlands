using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceABWithOneBonusReward : RewardBase
    {
        public ResourceId resourceIdA { get; set; } = ResourceId.Gold;
        public int amountA { get; set; }

        public ResourceId resourceIdB { get; set; } = ResourceId.Wood;
        public int amountB { get; set; }

        public int bonusA_min { get; set; }
        public int bonusA_max { get; set; }
        public int bonusB_min { get; set; }
        public int bonusB_max { get; set; }


        public override Task<string?> AddReward(GameSession session)
        {
            var isA = new Random().Next(2) == 0;
            return isA ? AddRewardWithBonusA(session) : AddRewardWithBonusB(session);
        }

        private Task<string?> AddRewardWithBonusA(GameSession session)
        {
            var amountWithBonus = amountA + new Random().Next(bonusA_min, bonusA_max + 1);
            var resourceDatas = new ResourceData[]
            {
                new ResourceData(resourceIdA, amountWithBonus),
                new ResourceData(resourceIdB, amountB),
            };
            session.player.resources.ForceAdd(resourceDatas);

            var text = resourceDatas.GetLocalizedView(session);
            return Task.FromResult(text);
        }

        private Task<string?> AddRewardWithBonusB(GameSession session)
        {
            var amountWithBonus = amountB + new Random().Next(bonusB_min, bonusB_max + 1);
            var resourceDatas = new ResourceData[]
            {
                new ResourceData(resourceIdA, amountA),
                new ResourceData(resourceIdB, amountWithBonus),
            };
            session.player.resources.ForceAdd(resourceDatas);

            var text = resourceDatas.GetLocalizedView(session);
            return Task.FromResult(text);
        }

    }
}
