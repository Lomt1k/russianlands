using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Arena;
public class ArenaMatchMaker : Service
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private Dictionary<LeagueId, ArenaLeagueMatchMaker> _leagueMatchMakers = new();
    private CancellationTokenSource _cts = new CancellationTokenSource();

    public override Task OnBotStarted()
    {
        _cts = new CancellationTokenSource();
        _leagueMatchMakers.Clear();
        foreach (var arenaLeagueSettings in gameDataHolder.arenaLeagueSettings.GetAllData())
        {
            _leagueMatchMakers[arenaLeagueSettings.id] = new ArenaLeagueMatchMaker(arenaLeagueSettings, _cts.Token);
        }
        return Task.CompletedTask;
    }

    public override Task OnBotStopped()
    {
        _cts.Cancel();
        _leagueMatchMakers.Clear();
        return Task.CompletedTask;
    }

    /// <returns>estimated time (or null if registration failed)</returns>
    public async Task<TimeSpan?> TryRegisterPlayer(Player player)
    {
        var leagueId = ArenaHelper.GetActualLeagueForPlayer(player);
        if (_leagueMatchMakers.TryGetValue(leagueId, out var leagueMatchMaker))
        {
            return await leagueMatchMaker.TryRegisterPlayer(player).FastAwait();
        }
        return null;
    }

    public async Task<bool> TryUnregisterPlayer(Player player)
    {
        var leagueId = ArenaHelper.GetActualLeagueForPlayer(player);
        if (_leagueMatchMakers.TryGetValue(leagueId, out var leagueMatchMaker))
        {
            return await leagueMatchMaker.TryUnregisterPlayer(player).FastAwait();
        }
        return false;
    }

}
