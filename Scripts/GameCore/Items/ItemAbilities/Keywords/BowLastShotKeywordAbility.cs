﻿using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class BowLastShotKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Последний выстрел: последняя стрела наносит двойной урон";
    public override AbilityType abilityType => AbilityType.BowLastShotKeyword;

    public override string GetView(GameSession session)
    {
        return Emojis.StatKeywordBowLastShot + Localizations.Localization.Get(session, "ability_bow_last_shot");
    }

}
