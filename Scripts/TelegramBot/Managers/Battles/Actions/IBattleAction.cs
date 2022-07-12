using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public enum BattleActionType { BuffOrHeal, Attack }

    public interface IBattleAction
    {
        public BattleActionType actionType { get; }

        public void ActionWithMineStats(UnitStats stats);
        public void ActionWithEnemyStats(UnitStats stats);
        public string? GetLocalization(GameSession session);
    }
}
