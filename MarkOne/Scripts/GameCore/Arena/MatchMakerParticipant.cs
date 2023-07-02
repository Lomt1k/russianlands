using MarkOne.Scripts.GameCore.Units;
using System;
using System.Linq;

namespace MarkOne.Scripts.GameCore.Arena;
internal class MatchMakerParticipant
{
    public Player player { get; }
    public long[] blockDbids { get; }
    public DateTime timeToStartFakePVP { get; }

    public MatchMakerParticipant(Player _player, TimeSpan timeUntilStartFakePVP)
    {
        player = _player;
        blockDbids = _player.profile.dynamicData.arenaProgress.EnsureNotNull().results.Select(x => x.dbid).ToArray();
        timeToStartFakePVP = DateTime.UtcNow.Add(timeUntilStartFakePVP);
    }

    public bool CanPlayWith(MatchMakerParticipant otherParticipant)
    {
        return !blockDbids.Contains(otherParticipant.player.dbid) && !otherParticipant.blockDbids.Contains(player.dbid);
    }

}
