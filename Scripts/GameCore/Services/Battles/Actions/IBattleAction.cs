using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Services.Battles.Actions
{
    public interface IBattleAction
    {
        public void ApplyActionWithMineStats(UnitStats stats);
        public void ApplyActionWithEnemyStats(UnitStats stats);
        public string GetHeader(GameSession session);
        public string GetDescription(GameSession session);
    }
}
