using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Managers;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    public class StatusCommand : CommandBase
    {
        public override bool isAdminCommand => true;

        public override async Task Execute(GameSession session, string[] args)
        {
            var sb = new StringBuilder();

            var pm = GlobalManagers.performanceManager;
            sb.AppendLine($"Status: <b>{pm.currentState}</b>");
            sb.AppendLine($"CPU: {PerformanceMonitor.cpuUsage:F1}%"
                + (pm.currentCpuState == PerformanceState.Normal ? string.Empty : $" ({pm.currentCpuState}") );
            sb.AppendLine($"RAM: {PerformanceMonitor.memoryUsage:F0} MB"
                + (pm.currentMemoryState == PerformanceState.Normal ? string.Empty : $" ({pm.currentMemoryState}"));

            sb.AppendLine();
            var allSessions = TelegramBot.instance.sessionManager.GetAllSessions();
            sb.AppendLine($"Sessions: {allSessions.Count}");
            var dtNow = DateTime.UtcNow;
            var minutesCheck = TelegramBot.instance.config.sessionTimeoutInMinutesWhenMemoryHighoad;
            var recentlyActive = allSessions.Where(x => (dtNow - x.lastActivityTime).TotalMinutes < minutesCheck).Count();
            sb.AppendLine($"Active in {minutesCheck} minutes: {recentlyActive}");
            recentlyActive = allSessions.Where(x => (dtNow - x.lastActivityTime).TotalMinutes < 5).Count();
            sb.AppendLine($"Now playing: {recentlyActive}");

            sb.AppendLine();
            sb.AppendLine("<b>Last activity:</b>");
            var orderedByActivity = allSessions.OrderByDescending(x => x.lastActivityTime).ToArray();
            for (int i = 0; i < orderedByActivity.Length && i < 10; i++)
            {
                var activeSession = orderedByActivity[i];
                sb.AppendLine($"{activeSession.lastActivityTime.ToLongTimeString()} | {activeSession.actualUser}");
            }

            await messageSender.SendTextMessage(session.chatId, sb.ToString());
        }
    }
}
