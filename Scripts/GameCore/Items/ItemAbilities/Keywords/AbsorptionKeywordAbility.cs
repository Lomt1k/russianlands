using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords
{
    public class AbsorptionKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Поглощение: Нанесённый вами урон вернётся вам в качестве здоровья";
        public override AbilityType abilityType => AbilityType.AbsorptionKeyword;

        public override string GetView(GameSession session)
        {
            return Localization.Get(session, "ability_absorption", chanceToSuccessPercentage);
        }
    }
}
