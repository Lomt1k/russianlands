using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Arena;
public static class ArenaHelper
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    public static void TryUpLeagueByFarmedChips(Player player)
    {
        var actualLeague = GetActualLeagueForPlayer(player);
        var profileData = player.session.profile.data;
        var farmedChips = profileData.lastArenaLeagueFarmedChips;
        var chipsLimit = gameDataHolder.arenaLeagueSettings[actualLeague].maxFarmedChips;
        if (chipsLimit > 0 && farmedChips >= chipsLimit)
        {
            profileData.lastArenaLeagueId = actualLeague + 1;
            profileData.lastArenaLeagueFarmedChips = 0;
        }
    }

    public static LeagueId GetActualLeagueForPlayer(Player player)
    {        
        var defaultLeague = GetDefaultLeague(player);
        var lastLeague = player.profile.data.lastArenaLeagueId;
        return lastLeague > defaultLeague ? lastLeague : defaultLeague;
    }    

    private static LeagueId GetDefaultLeague(Player player)
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
