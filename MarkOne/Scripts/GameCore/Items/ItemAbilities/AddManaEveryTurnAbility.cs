using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities;

public class AddManaEveryTurnAbility : ItemAbilityBase
{
    public override string debugDescription => "Каждый ход даёт дополнительное очко маны";

    public override AbilityType abilityType => AbilityType.AddManaEveryTurn;

    public sbyte manaValue;

    public override string GetView(GameSession session)
    {
        return chanceToSuccessPercentage >= 100
            ? Localizations.Localization.Get(session, "ability_add_mana_each_turn", manaValue)
            : Localizations.Localization.Get(session, "ability_add_mana_percentage_each_turn", chanceToSuccessPercentage, manaValue);
    }

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.Mana;
    }
}
