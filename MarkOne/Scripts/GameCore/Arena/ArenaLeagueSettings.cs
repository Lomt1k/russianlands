using MarkOne.Scripts.GameCore.Services.GameData;
using Newtonsoft.Json;
using System;

namespace MarkOne.Scripts.GameCore.Arena;

[JsonObject]
public class ArenaLeagueSettings : IGameDataWithId<LeagueId>
{
    public LeagueId id { get; set; }
    public byte maxPlayerLevel { get; set; }
    public byte maxSkillLevel { get; set; }
    public int maxFarmedChips { get; set; }
    public byte targetWins { get; set; }
    public byte generalMatchMakingTime { get; set; }
    public byte additionalMatchMakingTime { get; set; }
    public FakePlayerGenerationSettings defaultPlayerGenerationSettings { get; set; } = new();
    public FakePlayerGenerationSettings weakPlayerGenerationSettings { get; set; } = new();

    public void OnBotAppStarted()
    {
        // ignored
    }

    public TimeSpan GetRandomMatchmakingTime()
    {
        var seconds = generalMatchMakingTime + new Random().Next(additionalMatchMakingTime + 1);
        return TimeSpan.FromSeconds(seconds);
    }
}
