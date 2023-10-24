using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Quests.NextStageTriggers;
using MarkOne.Scripts.GameCore.Quests.StageActions;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Quests.QuestStages;

[JsonObject]
public class QuestStageWithTrigger : QuestStage
{
    public List<StageActionBase> questActions { get; set; } = new();
    public List<Tooltip> tooltips { get; set; } = new();
    public List<TriggerBase> nextStageTriggers { get; set; } = new();

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
    public string comment { get; set; } = "New Tooltip";
    public string dialogType { get; set; } = string.Empty;
    public string localizationKey { get; set; } = "dialog_tooltip_press_button";
    public int buttonId { get; set; } = -1;
    public int stageAfterButtonClick { get; set; } = -1;

    [JsonIgnore]
    public bool isTooltipForDialogPanel { get; private set; }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        isTooltipForDialogPanel = dialogType.Contains("Panel");
    }
}
