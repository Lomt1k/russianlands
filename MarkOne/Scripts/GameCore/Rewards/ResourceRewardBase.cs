using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public abstract class ResourceRewardBase : RewardBase
{
    public bool forceAdd { get; set; }
    public abstract Task<string?> AddRewardWithAddPossiblePremiumRewardToList(GameSession session, List<ResourceReward> possiblePremiumRewards);

    public ResourceRewardBase(bool _forceAdd = false)
    {
        forceAdd = _forceAdd;
    }
}
