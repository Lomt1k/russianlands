﻿using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class FinishingKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Добивание: Увеличивает урон, если атака будет смертельной";
    public override AbilityType abilityType => AbilityType.FinishingKeyword;

    public byte damageBonusPercentage;

    public override string ToString()
    {
        return debugDescription + $" (на {damageBonusPercentage}%)";
    }

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordFinishing + Localization.Get(session, "ability_finishing", damageBonusPercentage);
    }
}
