using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public class RestoreHealthAbility : ItemAbilityBase
    {
        public override string debugDescription => "Восстанавливает здоровье";

        public override AbilityType abilityType => AbilityType.RestoreHealth;

        public override ActivationType activationType => ActivationType.ByUser;


        public int healthValue;

        public override string ToString()
        {
            return chanceToSuccessPercentage >= 100
                ? $"Восставливает {healthValue} единиц здоровья"
                : $"С вероятностью {chanceToSuccessPercentage}% восстанавливает {healthValue} единиц здоровья";
        }

        public override string GetView(GameSession session)
        {
            return chanceToSuccessPercentage >= 100
                ? string.Format(Localizations.Localization.Get(session, "ability_restore_health"), healthValue)
                : string.Format(Localizations.Localization.Get(session, "ability_restore_health_percentage"), chanceToSuccessPercentage, healthValue);
        }

    }
}
