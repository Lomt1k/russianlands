using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class CompleteQuestAction : StageActionBase
    {
        [JsonProperty]
        public QuestId questId;

        public override async Task Execute(GameSession session)
        {
            var quest = QuestsHolder.GetQuest(questId);
            if (quest == null)
                return;

            await quest.CompleteQuest(session);
        }
    }
}
