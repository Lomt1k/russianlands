using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    // Используется только для вывода локализации, сам урон учитывается в ItemKeywordActionsHandler.cs
    public class RageAction : IBattleAction
    {
        public void ApplyActionWithMineStats(UnitStats stats)
        {
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
        }

        public string GetHeader(GameSession session)
        {
            return $"{Emojis.stats[Stat.KeywordRage]} {Localization.Get(session, "battle_action_rage_header")}";
        }

        public string GetDescription(GameSession session)
        {
            return Localization.Get(session, "battle_action_rage_description");
        }
        
    }
}
