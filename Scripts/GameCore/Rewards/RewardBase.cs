using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

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

public static class RewardBaseExtensions
{
    public static int GetInventoryItemsCount(this IEnumerable<RewardBase>? rewards)
    {
        if (rewards is null)
            return 0;

        var result = 0;
        foreach (var reward in rewards)
        {
            var inventoryItemsInReward = reward switch
            {
                ItemWithCodeReward => 1,
                RandomItemReward randomItemReward => randomItemReward.count,
                _ => 0
            };
            result += inventoryItemsInReward;
        }
        return result;
    }
}
