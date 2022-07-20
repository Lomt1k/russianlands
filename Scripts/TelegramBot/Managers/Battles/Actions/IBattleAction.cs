using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public enum BattleActionPriority { BeforeAttack, OnAttack }

    public interface IBattleAction
    {
        public BattleActionPriority priority { get; }

        public void ActionWithMineStats(UnitStats stats);
        public void ActionWithEnemyStats(UnitStats stats);
        public string? GetLocalization(GameSession session);
    }
}
