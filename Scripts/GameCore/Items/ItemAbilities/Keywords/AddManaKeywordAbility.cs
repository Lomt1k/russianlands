using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class AddManaKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Даёт дополнительное очко маны";
    public override AbilityType abilityType => AbilityType.AddManaKeyword;

    public override string GetView(GameSession session)
    {
        return Emojis.StatMana + Localization.Get(session, "ability_add_mana_percentage", chanceToSuccessPercentage);
    }
}
