using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    internal class RestoreManaAbility : ItemAbilityBase
    {
        public override string debugDescription => "Восстанавливает ману";

        public override AbilityType abilityType => AbilityType.RestoreMana;

        public override ActivationType activationType => ActivationType.ByUser;

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
                ? $"Восставливает {manaValue} единиц маны"
                : $"С вероятностью {chanceToSuccessPercentage}% восстанавливает {manaValue} единиц маны";
        }

        public override string GetView(GameSession session)
        {
            return chanceToSuccessPercentage >= 100
                ? string.Format(Localization.Localization.Get(session, "ability_restore_mana"), manaValue)
                : string.Format(Localization.Localization.Get(session, "ability_restore_mana_percentage"), chanceToSuccessPercentage, manaValue);
        }
    }
}
