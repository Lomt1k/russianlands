using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class EntryTownAction : StageActionBase
    {
        public override async Task Execute(GameSession session)
        {
            bool alreadyInTown = session.currentDialog is TownDialog;
            if (alreadyInTown)
                return;

            await new TownDialog(session, TownEntryReason.FromQuestAction).Start();
        }
    }
}
