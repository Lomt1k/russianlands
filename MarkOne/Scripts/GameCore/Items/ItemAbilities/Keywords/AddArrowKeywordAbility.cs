using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class AddArrowKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Лучник: Даёт дополнительную стрелу";

    public override AbilityType abilityType => AbilityType.AddArrowKeyword;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordAddArrow;
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordAddArrow +
            Localizations.Localization.Get(session, "ability_add_arrow_percentage", chanceToSuccessPercentage);
    }

}
