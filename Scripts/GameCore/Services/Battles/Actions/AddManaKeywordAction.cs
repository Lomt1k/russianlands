using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Services.Battles.Actions
{
    public class AddManaKeywordAction : IBattleAction
    {
        public void ApplyActionWithMineStats(UnitStats stats)
        {
            stats.currentMana++;
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
        }

        public string GetHeader(GameSession session)
        {
            return Emojis.StatMana + Localization.Get(session, "battle_action_add_mana_keyword_header");
        }

        public string GetDescription(GameSession session)
        {
            return Localization.Get(session, "battle_action_add_mana_keyword_description");
        }
    }
}
