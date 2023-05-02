using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Dialogs.Quests.MainQuest;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class ShowLanguageSelectionDialogAction : StageActionBase
{
    public override async Task Execute(GameSession session)
    {
        await new SelectLanguageDialog(session).Start();
    }
}
