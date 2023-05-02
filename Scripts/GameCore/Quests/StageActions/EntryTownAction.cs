using Newtonsoft.Json;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Dialogs.Town;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Services;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class EntryTownAction : StageActionBase
{
    private static readonly NotificationsManager notificationsManager = Services.Services.Get<NotificationsManager>();

    public override async Task Execute(GameSession session)
    {
        var alreadyInTown = session.currentDialog is TownDialog;
        if (alreadyInTown)
            return;

        await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.FromQuestAction).FastAwait();
    }
}
