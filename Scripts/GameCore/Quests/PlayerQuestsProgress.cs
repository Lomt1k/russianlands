using System.Collections.Generic;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    [JsonObject]
    public class PlayerQuestsProgress
    {
        [JsonProperty]
        private QuestType? focusedQuest = null; 

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

        public void SetStage(QuestType questType, int stage, bool isFocused)
        {
            stages[questType] = stage;
            focusedQuest = isFocused ? questType : (QuestType?)null;
        }

        public bool IsCompleted(QuestType questType)
        {
            return GetStage(questType) == -1;
        }

        public QuestType? GetFocusedQuest()
        {
            return focusedQuest;
        }

    }
}
