using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class AddArrowKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Лучник: Даёт дополнительную стрелу";

    public override AbilityType abilityType => AbilityType.AddArrowKeyword;

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordAddArrow +
            Localizations.Localization.Get(session, "ability_add_arrow_percentage", chanceToSuccessPercentage);
    }

}
