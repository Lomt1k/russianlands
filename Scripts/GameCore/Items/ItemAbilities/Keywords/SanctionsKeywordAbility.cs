using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords
{
    public class SanctionsKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Санкции: Соперник теряет стрелу или очко маны";
        public override AbilityType abilityType => AbilityType.SanctionsKeyword;

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.KeywordSanctions]} {Localization.Get(session, "ability_sanctions_percentage", chanceToSuccessPercentage)}";
        }
    }
}
