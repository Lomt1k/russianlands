using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    // Используется только для вывода локализации, сам урон учитывается в ItemKeywordActionsHandler.cs
    public class FinishingAction : IBattleAction
    {
        private byte _damageBonusPercentage;

        public FinishingAction(byte damageBonusPercentage)
        {
            _damageBonusPercentage = damageBonusPercentage;
        }

        public void ApplyActionWithMineStats(UnitStats stats)
        {
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
        }

        public string GetHeader(GameSession session)
        {
            return $"{Emojis.stats[Stat.KeywordFinishing]} {Localization.Get(session, "battle_action_finishing_header")}";
        }

        public string GetDescription(GameSession session)
        {
            return Localization.Get(session, "battle_action_finishing_description", _damageBonusPercentage);
        }

    }
}
