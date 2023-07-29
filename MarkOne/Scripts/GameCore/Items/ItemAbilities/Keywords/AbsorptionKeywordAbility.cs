using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class AbsorptionKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Поглощение: Нанесённый вами урон вернётся вам в качестве здоровья";
    public override AbilityType abilityType => AbilityType.AbsorptionKeyword;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordAbsorption;
    }

    public override string GetView(GameSession session)
    {
        return Localization.Get(session, "ability_absorption", chanceToSuccessPercentage);
    }
}
