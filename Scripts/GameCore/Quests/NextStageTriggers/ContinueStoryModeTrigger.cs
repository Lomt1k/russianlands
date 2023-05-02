using Newtonsoft.Json;

namespace MarkOne.Scripts.GameCore.Quests.NextStageTriggers;

[JsonObject]
public class ContinueStoryModeTrigger : TriggerBase
{
    public override TriggerType triggerType => TriggerType.ContinueStoryMode;

    public override bool TryInvoke()
    {
        return true;
    }
}
