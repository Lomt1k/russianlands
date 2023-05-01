using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class StartNewQuestAction : StageActionBase
    {
        [JsonProperty]
        public QuestId questId;

        public override async Task Execute(GameSession session)
        {
            var quest = QuestsHolder.GetQuest(questId);
            await quest.StartQuest(session);
        }
    }
}
