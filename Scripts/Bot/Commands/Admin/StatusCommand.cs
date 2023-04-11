﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Managers;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands.Admin
{
    public class StatusCommand : CommandBase
    {
        private static readonly PerformanceManager pm = Singletones.Get<PerformanceManager>();

        public override CommandGroup commandGroup => CommandGroup.Cheat;

        public override async Task Execute(GameSession session, string[] args)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Status: {pm.currentState.ToString().Bold()}");
            sb.AppendLine($"CPU: {PerformanceMonitor.cpuUsage:F1}%");
            sb.AppendLine($"RAM: {PerformanceMonitor.memoryUsage:F0} MB");

            sb.AppendLine();
            var allSessions = TelegramBot.instance.sessionManager.GetAllSessions();
            sb.AppendLine($"Active sessions: {allSessions.Count}");
            var dtNow = DateTime.UtcNow;
            var recentlyActive = allSessions.Where(x => (dtNow - x.lastActivityTime).TotalMinutes < 5).Count();
            sb.AppendLine($"Now playing: {recentlyActive}");

            var orderedByActivity = allSessions.OrderByDescending(x => x.lastActivityTime).ToArray();
            if (orderedByActivity.Length > 1)
            {
                sb.AppendLine();
                sb.AppendLine("Last activity:".Bold());
                int i = 0;
                foreach (var activeSession in orderedByActivity)
                {
                    if (session == activeSession)
                        continue;

                    var timeSpan = (dtNow - activeSession.lastActivityTime).GetShortView(session);
                    sb.AppendLine($"{timeSpan} | {activeSession.actualUser}");
                    i++;
                    if (i == 10)
                        break;
                }
            }            

            await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();
        }
    }
}
