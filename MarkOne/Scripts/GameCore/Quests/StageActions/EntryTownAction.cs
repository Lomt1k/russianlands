using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Dialogs.Town;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class EntryTownAction : StageActionBase
{
    private static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();

    public override async Task Execute(GameSession session)
    {
        var alreadyInTown = session.currentDialog is TownDialog;
        if (alreadyInTown)
            return;

        await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.FromQuestAction).FastAwait();
    }
}
