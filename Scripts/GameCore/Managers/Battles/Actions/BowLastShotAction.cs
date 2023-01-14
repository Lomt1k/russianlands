using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    // Используется только для вывода локализации, сам урон учитывается в ItemKeywordActionsHandler.cs
    public class BowLastShotAction : IBattleAction
    {
        public void ApplyActionWithMineStats(UnitStats stats)
        {
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
        }

        public string GetHeader(GameSession session)
        {
            return Emojis.StatKeywordBowLastShot + Localization.Get(session, "battle_action_bow_last_shot_header");
        }

        public string GetDescription(GameSession session)
        {
            return Localization.Get(session, "battle_action_bow_last_shot_description");
        }
        
    }
}
