using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Managers;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    public class StatsCommand : CommandBase
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
            sb.AppendLine($"Sessions: {TelegramBot.instance.sessionManager.sessionsCount}");

            await messageSender.SendTextMessage(session.chatId, sb.ToString());
        }
    }
}
