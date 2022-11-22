using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    public enum BattleActionPriority { BeforeAttack, OnAttack, AfterAttack }
    public enum ActivationType { EveryTurn, OnSelectItem }

    public interface IBattleAction
    {
        public BattleActionPriority priority { get; }
        public ActivationType activationType { get; }

        public void ApplyActionWithMineStats(UnitStats stats);
        public void ApplyActionWithEnemyStats(UnitStats stats);
        public string GetLocalization(GameSession session);
    }
}
