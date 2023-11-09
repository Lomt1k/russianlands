using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Rewards;
[JsonObject]
public class AvatarReward : RewardBase
{
    public AvatarId avatarId { get; set; }

    public AvatarReward(AvatarId _avatarId)
    {
        avatarId = _avatarId;
    }

    public AvatarReward() : this(AvatarId.Male_00)
    {
    }

    public override Task<string?> AddReward(GameSession session)
    {
        session.profile.dynamicData.avatars.Add(avatarId);
        var result = GetPossibleRewardsView(session);
        return Task.FromResult(result);
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        return avatarId.GetEmoji() + Localization.Get(session, "reward_avatar");
    }
}
