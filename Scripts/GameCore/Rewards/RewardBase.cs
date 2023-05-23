using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<RewardBase>))]
public abstract class RewardBase
{
    [JsonIgnore]
    protected static readonly MessageSender messageSender = Services.ServiceLocator.Get<MessageSender>();

    public abstract Task<string?> AddReward(GameSession session);
    public abstract string GetPossibleRewardsView(GameSession session);
}
