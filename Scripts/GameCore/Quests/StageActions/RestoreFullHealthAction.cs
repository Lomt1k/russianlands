using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class RestoreFullHealthAction : StageActionBase
{
    public override async Task Execute(GameSession session)
    {
        session.player.unitStats.SetFullHealth();
    }
}
