using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class EntryTownAction : StageActionBase
    {
        private static readonly NotificationsManager notificationsManager = Singletones.Get<NotificationsManager>();

        public override async Task Execute(GameSession session)
        {
            bool alreadyInTown = session.currentDialog is TownDialog;
            if (alreadyInTown)
                return;

            await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.FromQuestAction).FastAwait();
        }
    }
}
