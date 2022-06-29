using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public class RestoreHealthEveryTurnAbility : ItemAbilityBase
    {
        public override string debugDescription => "Восстанавливает здоровье (каждый ход)";

        public override AbilityType abilityType => AbilityType.RestoreHealthEveryTurn;

        public override ActivationType activationType => ActivationType.EveryTurn;

        public override bool isSupportLevelUp => true;

        public int healthValue;

        public override void ApplyItemLevel(byte level)
        {
            IncreaseByTenPercentByLevel(ref healthValue, level);
            IncreaseByTenPercentByLevel(ref manaCost, level);
        }

        public override string ToString()
        {
            return chanceToSuccessPercentage >= 100
                ? $"Каждый ход восставливает {healthValue} единиц здоровья"
                : $"Кадый ход с вероятностью {chanceToSuccessPercentage}% восстанавливает {healthValue} единиц здоровья";
        }

        public override string GetView(GameSession session)
        {
            return chanceToSuccessPercentage >= 100
                ? string.Format(Localizations.Localization.Get(session, "ability_restore_health_each_turn"), healthValue)
                : string.Format(Localizations.Localization.Get(session, "ability_restore_health_percentage_each_turn"), chanceToSuccessPercentage, healthValue);
        }
    }
}
