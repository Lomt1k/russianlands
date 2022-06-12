using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    internal class AddManaEveryTurnAbility : ItemAbilityBase
    {
        public override string debugDescription => "Добавляет дополнительное очко маны (каждый ход)";

        public override AbilityType abilityType => AbilityType.AddManaEveryTurn;

        public override ActivationType activationType => ActivationType.EveryTurn;

        public override bool isSupportLevelUp => false;

        public override void ApplyItemLevel(byte level) { }

        public override string ToString()
        {
            return chanceToSuccessPercentage >= 100
                ? $"Каждый ход дает дополнительное очко маны"
                : $"Кадый ход с вероятностью {chanceToSuccessPercentage}% дает дополнительное очко маны";
        }

        public override string GetView(GameSession session)
        {
            return chanceToSuccessPercentage >= 100
                ? Localizations.Localization.Get(session, "ability_add_mana_each_turn")
                : string.Format(Localizations.Localization.Get(session, "ability_add_mana_percentage_each_turn"), chanceToSuccessPercentage);
        }
    }
}
