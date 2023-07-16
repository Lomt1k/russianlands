using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Rewards;
public abstract class ResourceRewardBase : RewardBase
{
    public abstract Task<string?> AddRewardWithAddPossiblePremiumRewardToList(GameSession session, List<ResourceReward> possiblePremiumRewards);
}
