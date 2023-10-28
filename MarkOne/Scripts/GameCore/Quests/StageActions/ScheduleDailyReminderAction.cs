using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.DailyReminders;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class ScheduleDailyReminderAction : StageActionBase
{
    private static DailyRemindersManager dailyRemindersManager = ServiceLocator.Get<DailyRemindersManager>();

    public override async Task Execute(GameSession session)
    {
        await dailyRemindersManager.ScheduleReminderSequence(session.profile).FastAwait();
    }
}
