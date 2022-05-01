using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class SessionManager
    {
        private const int millisecondsInHour = 3_600_000;
        private const int millisecondsInMinute = 60_000;
        private readonly int sessionTimeoutInMilliseconds;
        private readonly int periodicSaveDatabaseInMinutes;

        private TelegramBot _telegramBot;
        private Dictionary<long, GameSession> _sessions = new Dictionary<long, GameSession>();

        public SessionManager(TelegramBot telegramBot)
        {
            _telegramBot = telegramBot;
            sessionTimeoutInMilliseconds = telegramBot.config.sessionTimeoutInHours * millisecondsInHour;
            periodicSaveDatabaseInMinutes = telegramBot.config.periodicSaveDatabaseInMinutes * millisecondsInMinute;

            Task.Run(() => PeriodicSaveProfilesAsync());
            Task.Run(() => CloseSessionsWithTimeoutAsync());
        }

        public GameSession GetOrCreateSession(User user)
        {
            if (!_sessions.TryGetValue(user.Id, out var session))
            {
                session = new GameSession(user);
                _sessions.Add(user.Id, session);
            }            
            return session;
        }

        public GameSession? GetSessionIfExists(long userId)
        {
            _sessions.TryGetValue(userId, out GameSession? session);
            return session;
        }

        public bool HasActiveSession(long userId)
        {
            return _sessions.ContainsKey(userId);
        }

        private async Task PeriodicSaveProfilesAsync()
        {
            while (true)
            {
                await Task.Delay(periodicSaveDatabaseInMinutes);
                Program.logger.Info("Saving changes in database for active users...");
                foreach (var session in _sessions.Values)
                {
                    session.SaveProfileIfNeed();
                }
            }
        }

        private async Task CloseSessionsWithTimeoutAsync()
        {
            while (true)
            {
                await Task.Delay(10_000);
                List<long> sessionsToClose = new List<long>();
                foreach (var userId in _sessions.Keys)
                {
                    if (IsTimeout(userId))
                    {
                        sessionsToClose.Add(userId);
                    }
                }
                foreach (var userId in sessionsToClose)
                {
                    CloseSession(userId);
                }
            }
        }

        private bool IsTimeout(long userId)
        {
            var session = _sessions[userId];
            var millisecondsFromLastActivity = (int)(DateTime.UtcNow - session.lastActivityTime).TotalMilliseconds;
            return millisecondsFromLastActivity > sessionTimeoutInMilliseconds;
        }

        public void CloseSession(long userId)
        {
            if (_sessions.TryGetValue(userId, out var session))
            {
                session.OnCloseSession();
                _sessions.Remove(userId);
            }
        }


    }
}
