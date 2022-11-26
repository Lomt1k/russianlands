using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public class StealManaAbility : ItemAbilityBase
    {
        public override string debugDescription => "Кража маны: Забирает очко маны у противника";

        public override AbilityType abilityType => AbilityType.StealManaKeyword;

        public override string ToString()
        {
            return $"{debugDescription} (Вероятность {chanceToSuccessPercentage}%)";
        }

        public override string GetView(GameSession session)
        {
            return Emojis.stats[Stat.KeywordStealMana] + ' ' +
                string.Format(Localizations.Localization.Get(session, "ability_steal_mana_percentage"), chanceToSuccessPercentage);
        }
    }
}
