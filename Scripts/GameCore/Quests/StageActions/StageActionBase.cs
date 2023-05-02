using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions;

[JsonConverter(typeof(JsonKnownTypesConverter<StageActionBase>))]
public abstract class StageActionBase
{
    public abstract Task Execute(GameSession session);
}
