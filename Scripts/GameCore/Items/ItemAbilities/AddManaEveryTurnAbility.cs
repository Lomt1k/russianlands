using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public class AddManaEveryTurnAbility : ItemAbilityBase
    {
        public override string debugDescription => "Дает дополнительное очко маны (каждый ход)";

        public override AbilityType abilityType => AbilityType.AddManaEveryTurn;

        public override ActivationType activationType => ActivationType.EveryTurn;

        public int manaValue;

        public override string ToString()
        {
            return chanceToSuccessPercentage >= 100
                ? $"Каждый ход добавляет {manaValue} к мане"
                : $"Кадый ход с вероятностью {chanceToSuccessPercentage}% добавляет {manaValue} к мане";
        }

        public override string GetView(GameSession session)
        {
            return chanceToSuccessPercentage >= 100
                ? string.Format(Localizations.Localization.Get(session, "ability_add_mana_each_turn"), manaValue)
                : string.Format(Localizations.Localization.Get(session, "ability_add_mana_percentage_each_turn"), chanceToSuccessPercentage, manaValue);
        }
    }
}
