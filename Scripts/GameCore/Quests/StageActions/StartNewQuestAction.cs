using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services.GameData;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions;

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
