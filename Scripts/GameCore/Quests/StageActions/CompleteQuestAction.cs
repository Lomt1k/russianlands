using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class CompleteQuestAction : StageActionBase
    {
        [JsonProperty]
        public QuestType questType;

        public override async Task Execute(GameSession session)
        {
            var quest = QuestsHolder.GetQuest(questType);
            if (quest == null)
                return;

            await quest.CompleteQuest(session);
        }
    }
}
