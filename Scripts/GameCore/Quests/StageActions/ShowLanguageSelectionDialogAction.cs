using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Quests.MainQuest;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class ShowLanguageSelectionDialogAction : StageActionBase
{
    public override async Task Execute(GameSession session)
    {
        await new SelectLanguageDialog(session).Start();
    }
}
