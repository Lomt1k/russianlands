
namespace TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers
{
    public abstract class NextStageTriggerBase
    {
        public abstract TriggerType triggerType { get; }
        public int nextStage = -1;
    }
}
