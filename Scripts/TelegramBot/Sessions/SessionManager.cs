using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class SessionManager
    {
        private const int millisecondsInHour = 3_600_000;
        private readonly int sessionTimeoutInMilliseconds;

        private TelegramBot _telegramBot;
        private Dictionary<long, GameSession> _sessions = new Dictionary<long, GameSession>();

        public SessionManager(TelegramBot telegramBot)
        {
            _telegramBot = telegramBot;
            sessionTimeoutInMilliseconds = telegramBot.config.sessionTimeoutInHours * millisecondsInHour;
        }

        public GameSession GetOrCreateSession(User user)
        {
            if (!_sessions.TryGetValue(user.Id, out var session))
            {
                session = new GameSession(user);
                _sessions.Add(user.Id, session);
                Task.Run(() => CloseSessionsWithTimeoutAsync());
            }            
            return session;
        }

        public bool HasActiveSession(long userId)
        {
            return _sessions.ContainsKey(userId);
        }

        private async void CloseSessionsWithTimeoutAsync()
        {
            while (true)
            {
                await Task.Delay(10_000);
                foreach (var kvp in _sessions)
                {
                    CloseSessionIfTimeout(kvp.Key);
                }
            }
            
        }

        private void CloseSessionIfTimeout(long userId)
        {
            var session = _sessions[userId];
            var millisecondsFromLastActivity = (int)(DateTime.UtcNow - session.lastActivityTime).TotalMilliseconds;
            if (millisecondsFromLastActivity > sessionTimeoutInMilliseconds)
            {
                CloseSession(userId);
            }
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
