using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Arena;
public static class ArenaHelper
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    public static LeagueId GetActualLeagueForPlayer(Player player)
    {
        var playerLevel = player.level;
        var skillLevel = player.skills.GetAverageSkillLevel();
        var allLeagues = gameDataHolder.arenaLeagueSettings.GetAllData();
        foreach (var leagueSettings in allLeagues)
        {
            if (playerLevel <= leagueSettings.maxPlayerLevel && skillLevel <= leagueSettings.maxSkillLevel)
            {
                return leagueSettings.id;
            }
        }
        throw new System.InvalidOperationException($"Not found LeagueId for (playerLevel: {playerLevel}, skillLevel: {skillLevel})");
    }
}
