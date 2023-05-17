using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Arena;
public class ArenaMatchMaker : Service
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    Dictionary<LeagueId, ArenaLeagueMatchMaker> _leagueMatchMakers = new();

    public override Task OnBotStarted()
    {
        _leagueMatchMakers.Clear();
        foreach (var arenaLeagueSettings in gameDataHolder.arenaLeagueSettings.GetAllData())
        {
            _leagueMatchMakers[arenaLeagueSettings.id] = new ArenaLeagueMatchMaker(arenaLeagueSettings);
        }
        return Task.CompletedTask;
    }

    public override Task OnBotStopped()
    {
        foreach (var leagueMatchMaker in _leagueMatchMakers.Values)
        {
            leagueMatchMaker.OnBotStopped();
        }
        _leagueMatchMakers.Clear();
        return Task.CompletedTask;
    }

    public bool TryRegisterPlayer(Player player, out TimeSpan estimatedTime)
    {
        estimatedTime = TimeSpan.Zero;
        var leagueId = ArenaHelper.GetActualLeagueForPlayer(player);
        if (_leagueMatchMakers.TryGetValue(leagueId, out var leagueMatchMaker))
        {
            return leagueMatchMaker.TryRegisterPlayer(player, out estimatedTime);
        }
        return false;
    }

    public bool TryUnregisterPlayer(Player player)
    {
        var leagueId = ArenaHelper.GetActualLeagueForPlayer(player);
        if (_leagueMatchMakers.TryGetValue(leagueId, out var leagueMatchMaker))
        {
            return leagueMatchMaker.TryUnregisterPlayer(player);
        }
        return false;
    }

}
