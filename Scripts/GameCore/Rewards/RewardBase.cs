using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    [JsonConverter(typeof(JsonKnownTypesConverter<RewardBase>))]
    public abstract class RewardBase
    {
        [JsonIgnore]
        protected static readonly MessageSender messageSender = Singletones.Get<MessageSender>();

        public abstract Task<string> AddReward(GameSession session);
    }
}
