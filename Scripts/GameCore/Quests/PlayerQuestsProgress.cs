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

        public void SetStage(QuestType questType, int stage)
        {
            stages[(ushort)questType] = stage;
            if (stage > 0)
            {
                focusedQuest = questType;
            }
            else if (focusedQuest == questType)
            {
                focusedQuest = null;
            }
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
