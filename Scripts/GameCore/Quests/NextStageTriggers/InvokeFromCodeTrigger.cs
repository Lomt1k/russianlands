using Newtonsoft.Json;

namespace MarkOne.Scripts.GameCore.Quests.NextStageTriggers;

[JsonObject]
public class InvokeFromCodeTrigger : TriggerBase
{
    public override TriggerType triggerType => TriggerType.InvokeFromCode;

    public override bool TryInvoke()
    {
        return true;
    }
}
