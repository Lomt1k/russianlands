using TextGameRPG.Scripts.GameCore.Quests.ActionsOnStartStage;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    internal class QuestStageWithEndingTrigger : QuestStage
    {
        public ActionType[]? actions { get; set; } = null;
        public int? nextStage { get; set; }
        public Tooltip[]? tooltips { get; set; }
        public EndStageTrigger? endStageTrigger { get; set; }
    }

    [JsonObject]
    internal class Tooltip
    {
        public string comment { get; set; } = "New Tooltip";
        public string dialogType { get; set; } = string.Empty;
        public string localizationKey { get; set; } = string.Empty;
    }


}
