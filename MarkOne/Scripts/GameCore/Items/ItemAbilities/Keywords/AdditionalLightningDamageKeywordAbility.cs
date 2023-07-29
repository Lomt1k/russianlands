using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class AdditionalLightningDamageKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Пламенный удар: Наносит дополнительный урон";
    public override AbilityType abilityType => AbilityType.AdditionalLightningDamageKeyword;

    public int damageAmount;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordAdditionalDamage;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{debugDescription} {chanceToSuccessPercentage}:");
        sb.AppendLine($"lightning: {damageAmount}");

        return sb.ToString();
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordAdditionalDamage +
            Localization.Get(session, "ability_extra_lightning_damage_percentage", chanceToSuccessPercentage, damageAmount);
    }

}
