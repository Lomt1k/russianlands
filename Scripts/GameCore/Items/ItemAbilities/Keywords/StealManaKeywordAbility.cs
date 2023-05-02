using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class StealManaKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Кража маны: Забирает очко маны у противника";

    public override AbilityType abilityType => AbilityType.StealManaKeyword;

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordStealMana +
            Localizations.Localization.Get(session, "ability_steal_mana_percentage", chanceToSuccessPercentage);
    }
}
