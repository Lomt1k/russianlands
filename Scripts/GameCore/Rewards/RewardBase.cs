using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<RewardBase>))]
public abstract class RewardBase
{
    [JsonIgnore]
    protected static readonly MessageSender messageSender = Services.Services.Get<MessageSender>();

    public abstract Task<string?> AddReward(GameSession session);
}
