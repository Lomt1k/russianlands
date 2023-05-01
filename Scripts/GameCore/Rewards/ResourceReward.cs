﻿using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceReward : RewardBase
    {
        public ResourceId resourceId { get; set; } = ResourceId.Gold;
        public int amount { get; set; }

        public override Task<string> AddReward(GameSession session)
        {
            session.player.resources.ForceAdd(resourceId, amount);
            var result = resourceId.GetLocalizedView(session, amount);
            return Task.FromResult(result);
        }
    }
}
