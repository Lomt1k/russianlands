using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers
{
    [JsonObject]
    internal class OnDialogCloseTrigger : TriggerBase
    {
        public override TriggerType triggerType => TriggerType.OnDialogClose;

        public override bool TryInvoke()
        {
            return true;
        }
    }
}
