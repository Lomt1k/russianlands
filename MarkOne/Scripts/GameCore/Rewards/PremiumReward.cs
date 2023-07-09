using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Rewards;
[JsonObject]
public class PremiumReward : RewardBase
{
    public int seconds { get; set; }

    [JsonIgnore]
    public TimeSpan timeSpan { get; set; }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        timeSpan = TimeSpan.FromSeconds(seconds);
    }

    public override Task<string> AddReward(GameSession session)
    {
        var profileData = session.profile.data;
        profileData.endPremiumTime = profileData.IsPremiumActive()
            ? profileData.endPremiumTime.Add(timeSpan)
            : DateTime.UtcNow.Add(timeSpan);

        var result = GetPossibleRewardsView(session);
        return Task.FromResult(result);
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        return Emojis.StatPremium + Localization.Get(session, "premium_reward", timeSpan.GetShortView(session));
    }

    


}
