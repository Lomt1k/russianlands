using MarkOne.Scripts.GameCore.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Arena;
public class ArenaLeagueMatchMaker
{
    private readonly ArenaLeagueSettings _arenaLeagueSettings;
    private readonly CancellationToken _cancellationToken;

    private TimeSpan _estimatedTime;
    private List<MatchMakerParticipant> _participants = new();
    private bool _isRegistrationBlocked;

    public ArenaLeagueMatchMaker(ArenaLeagueSettings arenaLeagueSettings, CancellationToken cancellationToken)
    {
        _arenaLeagueSettings = arenaLeagueSettings;
        _cancellationToken = cancellationToken;
        Task.Run(PeriodicRefreshEstimatedTime, cancellationToken);
        Task.Run(PeriodicTryCreateBattles, cancellationToken);
    }

    private async Task PeriodicRefreshEstimatedTime()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            _estimatedTime = _arenaLeagueSettings.GetRandomMatchmakingTime();
            await Task.Delay(30_000).FastAwait();
        }
    }
    
    private async Task PeriodicTryCreateBattles()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            TryCreateBattles();
            await Task.Delay(1000).FastAwait();
        }
    }

    private void TryCreateBattles()
    {
        _isRegistrationBlocked = true;
        {
            var now = DateTime.UtcNow;
            for (int i = 0; i < _participants.Count; i++)
            {
                var participantA = _participants[i];
                if (TryFindOpponent(i, out int j))
                {
                    var participantB = _participants[j];
                    // TODO: Start Battle (participantA vs participantB)
                    _participants.RemoveAt(j);
                    _participants.RemoveAt(i);
                    i--;
                }
                else if (participantA.timeToStartFakePVP <= now)
                {
                    // TODO: Start Battle (participantA vs fakePlayer)
                    _participants.RemoveAt(i);
                    i--;
                }
            }
        }
        _isRegistrationBlocked = false;
    }

    private bool TryFindOpponent(int mineIndex, out int opponentIndex)
    {
        opponentIndex = -1;
        var mineParticipant = _participants[mineIndex];
        for (int i = mineIndex + 1; i < _participants.Count; i++)
        {
            var opponent = _participants[i];
            if (opponent.CanPlayWith(mineParticipant))
            {
                opponentIndex = i;
                return true;
            }
        }
        return false;
    }

    public async Task<TimeSpan> TryRegisterPlayer(Player player)
    {
        var timeToStartFakePVP = _arenaLeagueSettings.GetRandomMatchmakingTime();
        var participant = new MatchMakerParticipant(player, timeToStartFakePVP);
        await WaitForRegistrationUnlock().FastAwait();
        {
            _participants.Add(participant);
        }
        return _estimatedTime;
    }

    public async Task<bool> TryUnregisterPlayer(Player player)
    {
        await WaitForRegistrationUnlock().FastAwait();
        {
            var participant = _participants.FirstOrDefault(x => x.player == player);
            return participant != null && _participants.Remove(participant);
        }
    }

    private async Task WaitForRegistrationUnlock()
    {
        while (_isRegistrationBlocked)
        {
            await Task.Delay(15).FastAwait();
        }
    }

}
