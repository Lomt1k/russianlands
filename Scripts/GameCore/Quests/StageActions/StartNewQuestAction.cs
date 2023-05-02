using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Services.GameData;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class StartNewQuestAction : StageActionBase
{
    private static readonly GameDataHolder gameDataHolder = Services.Services.Get<GameDataHolder>();

    [JsonProperty]
    public QuestId questId;

    public override async Task Execute(GameSession session)
    {
        var quest = gameDataHolder.quests[questId];
        await quest.StartQuest(session);
    }
}
