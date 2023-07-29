using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class FinishingKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Добивание: Увеличивает урон, если атака будет смертельной";
    public override AbilityType abilityType => AbilityType.FinishingKeyword;

    public byte damageBonusPercentage;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordFinishing;
    }

    public override string ToString()
    {
        return debugDescription + $" (на {damageBonusPercentage}%)";
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordFinishing + Localization.Get(session, "ability_finishing", damageBonusPercentage);
    }
}
