﻿using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    public class StealManaAction : IBattleAction
    {
        public void ApplyActionWithMineStats(UnitStats stats)
        {
            stats.AddMana(1);
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
            stats.RemoveMana(1);
        }

        public string GetHeader(GameSession session)
        {
            return $"{Emojis.stats[Stat.KeywordStealMana]} {Localization.Get(session, "battle_action_mana_steal_header")}";
        }

        public string GetDescription(GameSession session)
        {
            return Localization.Get(session, "battle_action_mana_steal_description");
        }
        
    }
}
