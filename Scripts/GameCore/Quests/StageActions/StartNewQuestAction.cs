using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class StartNewQuestAction : StageActionBase
    {
        [JsonProperty]
        public QuestType questType;

        public override async Task Execute(GameSession session)
        {
            var quest = QuestsHolder.GetQuest(questType);
            await quest.StartQuest(session);
        }
    }
}
