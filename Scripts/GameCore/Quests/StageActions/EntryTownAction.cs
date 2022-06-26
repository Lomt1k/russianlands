using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class EntryTownAction : StageActionBase
    {
        public override async Task Execute(GameSession session)
        {
            await new TownEntryDialog(session, TownEntryReason.FromQuestAction).Start();
        }
    }
}
