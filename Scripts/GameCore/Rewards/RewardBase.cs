using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    [JsonConverter(typeof(JsonKnownTypesConverter<RewardBase>))]
    public abstract class RewardBase
    {
        public abstract Task<string> AddReward(GameSession session);
    }
}
