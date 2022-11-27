using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords
{
    public class RageKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Каждая третья атака наносит двойной урон";
        public override AbilityType abilityType => AbilityType.RageKeyword;

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.KeywordRage]} {Localizations.Localization.Get(session, "ability_rage")}";
        }
    }
}
