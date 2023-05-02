using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;

[JsonObject]
public class StartNextStageImmediateTrigger : TriggerBase
{
    public override TriggerType triggerType => TriggerType.StartNextStageImmediate;

    public override bool TryInvoke()
    {
        return true;
    }
}
