using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithReplica : QuestStage
    {
        public Replica replica { get; set; } = new Replica();

        public override Task InvokeStage(GameSession session)
        {
            //TODO
            return Task.CompletedTask;
        }
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
