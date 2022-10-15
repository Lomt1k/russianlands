using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers
{
    [JsonObject]
    public class ContinueStoryModeTrigger : TriggerBase
    {
        public override TriggerType triggerType => TriggerType.ContinueStoryMode;

        public override bool TryInvoke()
        {
            return true;
        }
    }
}
