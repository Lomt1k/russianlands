using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords
{
    public class AdditionalColdDamageKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Леденящий удар: Наносит дополнительный урон";
        public override AbilityType abilityType => AbilityType.AdditionalColdDamageKeyword;

        public int damageAmount;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{debugDescription} {chanceToSuccessPercentage}:");
            sb.AppendLine($"cold: {damageAmount}");

            return sb.ToString();
        }

        public override string GetView(GameSession session)
        {
            return Emojis.StatKeywordAdditionalDamage +
                Localization.Get(session, "ability_extra_cold_damage_percentage", chanceToSuccessPercentage, damageAmount);
        }

    }
}
