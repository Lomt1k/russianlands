using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class RageKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Каждая третья атака наносит двойной урон";
    public override AbilityType abilityType => AbilityType.RageKeyword;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordRage;
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordRage + Localizations.Localization.Get(session, "ability_rage");
    }
}
