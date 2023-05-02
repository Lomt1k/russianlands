using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Dialogs.Quests.MainQuest;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class ShowEnterNameDialogAction : StageActionBase
{
    public override async Task Execute(GameSession session)
    {
        await new EnterNameDialog(session).StartFromQuest();
    }
}
