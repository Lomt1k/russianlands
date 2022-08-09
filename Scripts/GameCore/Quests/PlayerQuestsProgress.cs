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
        private readonly Dictionary<ushort, int> stages = new Dictionary<ushort, int>();

        public bool IsStarted(QuestType questType)
        {
            return stages.ContainsKey((ushort)questType);
        }

        public int GetStage(QuestType questType)
        {
            return IsStarted(questType) ? stages[(ushort)questType] : 0;
        }

        public void SetStage(QuestType questType, int stage, bool isFocused)
        {
            stages[(ushort)questType] = stage;
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
