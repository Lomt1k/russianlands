using JsonKnownTypes;
using Newtonsoft.Json;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    [JsonConverter(typeof(JsonKnownTypesConverter<RewardBase>))]
    public abstract class RewardBase
    {
        public abstract void AddReward(Player player);
        public abstract string GetRewardView(GameSession session);
    }
}
