using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class StartNewQuestAction : StageActionBase
{
    private static readonly GameDataHolder gameDataHolder = Services.ServiceLocator.Get<GameDataHolder>();

    [JsonProperty]
    public QuestId questId { get; set; }

    public override async Task Execute(GameSession session)
    {
        var quest = gameDataHolder.quests[questId];
        await quest.StartQuest(session);
    }
}
