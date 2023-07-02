using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class CompleteQuestAction : StageActionBase
{
    private static readonly GameDataHolder gameDataHolder = Services.ServiceLocator.Get<GameDataHolder>();

    [JsonProperty]
    public QuestId questId;

    public override async Task Execute(GameSession session)
    {
        var quest = gameDataHolder.quests[questId];
        if (quest == null)
            return;

        await quest.CompleteQuest(session);
    }
}
