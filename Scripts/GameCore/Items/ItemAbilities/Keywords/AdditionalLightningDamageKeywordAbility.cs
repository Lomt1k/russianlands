using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords
{
    public class AdditionalLightningDamageKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Пламенный удар: Наносит дополнительный урон";
        public override AbilityType abilityType => AbilityType.AdditionalLightningDamageKeyword;

        public int damageAmount;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{debugDescription} {chanceToSuccessPercentage}:");
            sb.AppendLine($"lightning: {damageAmount}");

            return sb.ToString();
        }

        public override string GetView(GameSession session)
        {
            return Emojis.stats[Stat.KeywordAdditionalDamage] + ' ' +
                Localization.Get(session, "ability_extra_lightning_damage_percentage", chanceToSuccessPercentage, damageAmount);
        }

    }
}
