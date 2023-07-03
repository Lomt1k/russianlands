using JsonKnownTypes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonConverter(typeof(JsonKnownTypesConverter<StageActionBase>))]
public abstract class StageActionBase
{
    public abstract Task Execute(GameSession session);
}
