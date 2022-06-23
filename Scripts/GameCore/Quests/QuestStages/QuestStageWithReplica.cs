using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Quests.Characters;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithReplica : QuestStage
    {
        public Replica replica { get; set; } = new Replica();
    }

    [JsonObject]
    public class Replica
    {
        public CharacterType characterType { get; set; } = CharacterType.None;
        public string localizationKey { get; set; } = string.Empty;
        public List<Answer> answers { get; set; } = new List<Answer>();
    }

    [JsonObject]
    public class Answer
    {
        public string comment { get; set; } = "New Answer";
        public string localizationKey { get; set; } = string.Empty;
        public int nextStage { get; set; }
    }


}
