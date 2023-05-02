using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;

[JsonObject]
public class InvokeFromCodeTrigger : TriggerBase
{
    public override TriggerType triggerType => TriggerType.InvokeFromCode;

    public override bool TryInvoke()
    {
        return true;
    }
}
