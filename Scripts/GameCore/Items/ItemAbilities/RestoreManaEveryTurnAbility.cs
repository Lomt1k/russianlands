using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    internal class RestoreManaEveryTurnAbility : ItemAbilityBase
    {
        public override string debugDescription => "Восстанавливает ману (каждый ход)";

        public override AbilityType abilityType => AbilityType.RestoreManaEveryTurn;

        public override ActivationType activationType => ActivationType.EveryTurn;

        public override bool isSupportLevelUp => true;

        public int manaValue;

        public override void ApplyItemLevel(byte level)
        {
            IncreaseByTenPercentByLevel(ref manaValue, level);
            IncreaseByTenPercentByLevel(ref manaCost, level);
        }

        public override string ToString()
        {
            return chanceToSuccessPercentage >= 100
                ? $"Каждый ход восставливает {manaValue} единиц маны"
                : $"Кадый ход с вероятностью {chanceToSuccessPercentage}% восстанавливает {manaValue} единиц маны";
        }

        public override string GetView(GameSession session)
        {
            return chanceToSuccessPercentage >= 100
                ? string.Format(Localizations.Localization.Get(session, "ability_restore_mana_each_turn"), manaValue)
                : string.Format(Localizations.Localization.Get(session, "ability_restore_mana_percentage_each_turn"), chanceToSuccessPercentage, manaValue);
        }
    }
}
