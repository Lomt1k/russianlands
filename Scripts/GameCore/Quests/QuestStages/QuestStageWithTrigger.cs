using TextGameRPG.Scripts.GameCore.Quests.StageActions;
using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using System.Runtime.Serialization;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithTrigger : QuestStage
    {
        public List<StageActionBase> questActions { get; set; } = new List<StageActionBase>();
        public List<Tooltip> tooltips { get; set; } = new List<Tooltip>();
        public List<TriggerBase> nextStageTriggers { get; set; } = new List<TriggerBase>();

        public override async Task InvokeStage(GameSession session)
        {
            session.tooltipController.SetupTooltips(tooltips);
            foreach (var action in questActions)
            {
                await action.Execute(session);
            }

            if (nextStageTriggers.Count == 1 && nextStageTriggers[0].triggerType == TriggerType.StartNextStageImmediate)
            {
                //without await!
                QuestManager.TryInvokeTrigger(session, TriggerType.StartNextStageImmediate);
            }
        }

    }

    [JsonObject]
    public class Tooltip
    {
        public string comment = "New Tooltip";
        public string dialogType = string.Empty;
        public string localizationKey = "dialog_tooltip_press_button";
        public int buttonId = -1;
        public int stageAfterButtonClick = -1;

        [JsonIgnore]
        public bool isTooltipForDialogPanel { get; private set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            isTooltipForDialogPanel = dialogType.Contains("Panel");
        }
    }


}
