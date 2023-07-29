using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class SanctionsKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Санкции: Соперник теряет стрелу или очко маны";
    public override AbilityType abilityType => AbilityType.SanctionsKeyword;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordSanctions;
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordSanctions + Localization.Get(session, "ability_sanctions_percentage", chanceToSuccessPercentage);
    }
}
