using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class StealManaKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Кража маны: Забирает очко маны у противника";

    public override AbilityType abilityType => AbilityType.StealManaKeyword;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordStealMana;
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordStealMana +
            Localizations.Localization.Get(session, "ability_steal_mana_percentage", chanceToSuccessPercentage);
    }
}
