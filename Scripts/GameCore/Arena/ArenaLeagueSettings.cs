using MarkOne.Scripts.GameCore.Services.GameData;
using Newtonsoft.Json;

namespace MarkOne.Scripts.GameCore.Arena;

[JsonObject]
public class ArenaLeagueSettings : IGameDataWithId<LeagueId>
{
    public LeagueId id { get; set; }
    public byte maxPlayerLevel { get; set; }
    public byte maxSkillLevel { get; set; }
    public byte targetWins { get; set; }
    public byte generalMatchMakingTime { get; set; }
    public byte additionalMatchMakingTime { get; set; }

    public void OnSetupAppMode(AppMode appMode)
    {
        // ignored
    }
}
