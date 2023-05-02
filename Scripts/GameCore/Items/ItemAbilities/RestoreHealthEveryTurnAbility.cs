using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities;

public class RestoreHealthEveryTurnAbility : ItemAbilityBase
{
    public override string debugDescription => "Восстанавливает здоровье (каждый ход)";

    public override AbilityType abilityType => AbilityType.RestoreHealthEveryTurn;

    public int healthValue;

    public override string ToString()
    {
        return chanceToSuccessPercentage >= 100
            ? $"Каждый ход восставливает {healthValue} единиц здоровья"
            : $"Кадый ход с вероятностью {chanceToSuccessPercentage}% восстанавливает {healthValue} единиц здоровья";
    }

    public override string GetView(GameSession session)
    {
        return chanceToSuccessPercentage >= 100
            ? Localizations.Localization.Get(session, "ability_restore_health_each_turn", healthValue)
            : Localizations.Localization.Get(session, "ability_restore_health_percentage_each_turn", chanceToSuccessPercentage, healthValue);
    }
}
