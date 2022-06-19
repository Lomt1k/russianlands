using System.Collections.Generic;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    [JsonObject]
    public class PlayerQuestsProgress
    {
        [JsonProperty]
        private readonly Dictionary<QuestType, int> stages = new Dictionary<QuestType, int>();

        public bool IsStarted(QuestType questType)
        {
            return stages.ContainsKey(questType);
        }

        public int GetStage(QuestType questType)
        {
            return IsStarted(questType) ? stages[questType] : 0;
        }

        public void SetStage(QuestType questType, int stage)
        {
            stages[questType] = stage;
        }

        public bool IsFinished(QuestType questType)
        {
            return GetStage(questType) == -1;
        }

    }
}
