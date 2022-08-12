using Newtonsoft.Json;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ResourceReward : RewardBase
    {
        [JsonProperty]
        public ResourceType resourceType = ResourceType.Gold;
        [JsonProperty]
        public int amount;

        public override void AddReward(GameSession session)
        {
            session.player.resources.ForceAdd(resourceType, amount);
        }

        public override string GetRewardView(GameSession session)
        {
            return resourceType.GetLocalizedView(session, amount);
        }
    }
}
