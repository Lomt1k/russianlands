using TextGameRPG.Scripts.GameCore.Quests.StageActions;
using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithTrigger : QuestStage
    {
        public bool focusRequired = false;
        public List<StageActionBase> questActions { get; set; } = new List<StageActionBase>();
        public List<Tooltip> tooltips { get; set; } = new List<Tooltip>();
        public List<NextStageTriggerBase> nextStageTriggers { get; set; } = new List<NextStageTriggerBase>();
    }

    [JsonObject]
    public class Tooltip
    {
        public string comment { get; set; } = "New Tooltip";
        public string dialogType { get; set; } = string.Empty;
        public string localizationKey { get; set; } = string.Empty;
    }


}
