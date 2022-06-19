using Newtonsoft.Json;
using TextGameRPG.Scripts.GameCore.Quests.Characters;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    internal class QuestStageWithReplica : QuestStage
    {
        public Replica? replica { get; set; }
    }

    [JsonObject]
    internal class Replica
    {
        public CharacterType characterType { get; set; }
        public string localizationKey { get; set; } = string.Empty;
        public Answer[] answers { get; set; } = new Answer[0];
    }

    [JsonObject]
    internal class Answer
    {
        public string comment { get; set; } = "New Answer";
        public string localizationKey { get; set; } = string.Empty;
        public int nextStage { get; set; }
    }


}
