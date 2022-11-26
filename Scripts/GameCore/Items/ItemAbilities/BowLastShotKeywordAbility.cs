using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public class BowLastShotKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Последний выстрел: последняя стрела наносит двойной урон";
        public override AbilityType abilityType => AbilityType.BowLastShotKeyword;

        public override string ToString()
        {
            return debugDescription;
        }

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.KeywordBowLastShot]} {Localizations.Localization.Get(session, "ability_bow_last_shot")}";
        }

    }
}
