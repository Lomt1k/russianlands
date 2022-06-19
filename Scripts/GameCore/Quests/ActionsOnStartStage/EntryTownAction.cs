using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.ActionsOnStartStage
{
    internal class EntryTownAction : ActionOnStartStageBase
    {
        public override async Task Execute(GameSession session)
        {
            await new TownEntryDialog(session, TownEntryReason.BackFromInnerDialog).Start();
        }
    }
}
