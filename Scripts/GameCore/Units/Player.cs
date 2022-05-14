using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Player : IBattleUnit
    {
        public GameSession session { get; private set; }
        public UnitStats unitStats { get; private set; }
        public string nickname => session.profile.data.nickname;

        public Player(GameSession _session)
        {
            session = _session;
            unitStats = new PlayerStats(this);
        }
    }
}
