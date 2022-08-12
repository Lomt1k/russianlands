using JsonKnownTypes;
using Newtonsoft.Json;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    [JsonConverter(typeof(JsonKnownTypesConverter<RewardBase>))]
    public abstract class RewardBase
    {
        public abstract void AddReward(GameSession session);
        public abstract string GetRewardView(GameSession session);
    }
}
