using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class StunKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Заставляет противника пропустить следующий ход";
    public override AbilityType abilityType => AbilityType.StunKeyword;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordStun;
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordStun + Localization.Get(session, "ability_stun_percentage", chanceToSuccessPercentage);
    }
}
