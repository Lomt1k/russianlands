using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.DailyBonus;

[JsonObject]
public class DailyBonusData : IGameDataWithId<byte>
{
    public byte id { get; set; }
    public List<RewardBase> rewards { get; set; } = new();

    public DailyBonusData(byte _id)
    {
        id = _id;
    }

    public void OnBotAppStarted()
    {
        // ignored
    }

    public bool IsClaimed(GameSession session)
    {
        return session.profile.data.lastDailyBonusId >= id;
    }

    public string GetView(GameSession session)
    {
        return IsClaimed(session)
            ? Emojis.ElementCheckMarkBlack + rewards.GetPossibleRewardsView(session)
            : $"{id}. " + rewards.GetPossibleRewardsView(session);
    }
}
