using MarkOne.Scripts.GameCore.Units;
using System;

namespace MarkOne.Scripts.GameCore.Arena;
public class ArenaLeagueMatchMaker
{
    private readonly ArenaLeagueSettings _arenaLeagueSettings;

    public ArenaLeagueMatchMaker(ArenaLeagueSettings arenaLeagueSettings)
    {
        _arenaLeagueSettings = arenaLeagueSettings;
    }

    public bool TryRegisterPlayer(Player player, out TimeSpan estimatedTime)
    {
        //TODO
        estimatedTime = TimeSpan.Zero;
        return true;
    }

    public bool TryUnregisterPlayer(Player player)
    {
        // TODO
        return true;
    }

    public void OnBotStopped()
    {
        // TODO
    }
}
