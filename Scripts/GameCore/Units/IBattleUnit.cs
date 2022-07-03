using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public interface IBattleUnit
    {
        public string nickname { get; }
        public UnitStats unitStats { get; }
        public GameSession session { get; }

        public string GetStartTurnView(GameSession session);
    }
}
