using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public class AddArrowAbility : ItemAbilityBase
    {
        public override string debugDescription => "Лучник: Даёт дополнительную стрелу";

        public override AbilityType abilityType => AbilityType.AddArrowKeyword;

        public override string ToString()
        {
            return $"{debugDescription} (Вероятность {chanceToSuccessPercentage}%)";
        }

        public override string GetView(GameSession session)
        {
            return Emojis.stats[Stat.KeywordAddArrow] + ' ' +
                string.Format(Localizations.Localization.Get(session, "ability_add_arrow_percentage"), chanceToSuccessPercentage);
        }

    }
}
