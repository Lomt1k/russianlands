using System;
using TextGameRPG.Scripts.GameCore.Quests.ActionsOnStartStage;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [Serializable]
    internal class QuestStageWithEndingTrigger : QuestStage
    {
        public ActionType[]? actions = null;
        public int? nextStage;
        public Tooltip[]? tooltips = null;
        public EndStageTrigger? endStageTrigger = null;
    }

    [Serializable]
    internal class Tooltip
    {
        public string comment = string.Empty;
        public string dialogType = string.Empty;
        public string localizationKey = string.Empty;
    }


}
