using TextGameRPG.Scripts.GameCore.Quests.QuestActions;
using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithTrigger : QuestStage
    {
        public bool isFocusRequired { get; set; } = false;
        public List<StageActionBase> questActions { get; set; } = new List<StageActionBase>();
        public List<Tooltip> tooltips { get; set; } = new List<Tooltip>();
        public List<NextStageTriggerBase> nextStageTriggers { get; set; } = new List<NextStageTriggerBase>();
    }

    [JsonObject]
    public class Tooltip
    {
        public string comment = "New Tooltip";
        public string dialogType = string.Empty;
        public string localizationKey = string.Empty;
    }


}
