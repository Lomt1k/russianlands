using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords
{
    public class StunKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Заставляет противника пропустить следующий ход";
        public override AbilityType abilityType => AbilityType.StunKeyword;

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.KeywordStun]} {Localization.Get(session, "ability_stun_percentage", chanceToSuccessPercentage)}";
        }
    }
}
