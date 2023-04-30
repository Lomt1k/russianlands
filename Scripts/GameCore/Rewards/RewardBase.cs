using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    [JsonConverter(typeof(JsonKnownTypesConverter<RewardBase>))]
    public abstract class RewardBase
    {
        [JsonIgnore]
        protected static readonly MessageSender messageSender = Services.Services.Get<MessageSender>();

        public abstract Task<string?> AddReward(GameSession session);
    }
}
