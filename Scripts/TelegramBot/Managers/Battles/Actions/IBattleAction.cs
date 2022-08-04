using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public enum BattleActionPriority { BeforeAttack, OnAttack, AfterAttack }

    public interface IBattleAction
    {
        public BattleActionPriority priority { get; }

        public void ApplyActionWithMineStats(UnitStats stats);
        public void ApplyActionWithEnemyStats(UnitStats stats);
        public string GetLocalization(GameSession session);
    }
}
