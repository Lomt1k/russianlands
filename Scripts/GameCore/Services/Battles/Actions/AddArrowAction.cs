﻿using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Services.Battles.Actions;

public class AddArrowAction : IBattleAction
{
    public void ApplyActionWithMineStats(UnitStats stats)
    {
        stats.currentArrows++;
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordAddArrow + Localization.Get(session, "battle_action_add_arrow_header");
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_add_arrow_description");
    }

}
