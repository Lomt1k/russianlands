using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.TelegramBot.Managers;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class SessionManager
    {
        private const int millisecondsInHour = 3_600_000;
        private const int millisecondsInMinute = 60_000;
        private readonly int periodicSaveDatabaseInMinutes;

        private TelegramBot _telegramBot;
        private PerformanceManager _performanceManager;
        private CancellationTokenSource _sessionManagerTasksCTS;
        private Dictionary<ChatId, GameSession> _sessions = new Dictionary<ChatId, GameSession>();

        public int sessionsCount => _sessions.Count;

        public SessionManager(TelegramBot telegramBot)
        {
            _telegramBot = telegramBot;
            _performanceManager = GlobalManagers.performanceManager;
            periodicSaveDatabaseInMinutes = telegramBot.config.periodicSaveDatabaseInMinutes * millisecondsInMinute;

            _sessionManagerTasksCTS = new CancellationTokenSource();
            Task.Run(() => PeriodicSaveProfilesAsync(), _sessionManagerTasksCTS.Token);
            Task.Run(() => CloseSessionsWithTimeoutAsync(), _sessionManagerTasksCTS.Token);
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

        public GameSession? GetSessionIfExists(User user)
        {
            _sessions.TryGetValue(user.Id, out GameSession? session);
            return session;
        }

        public bool HasActiveSession(User user)
        {
            return _sessions.ContainsKey(user.Id);
        }

        private async Task PeriodicSaveProfilesAsync()
        {
            await Task.Delay(periodicSaveDatabaseInMinutes);
            while (!_sessionManagerTasksCTS.IsCancellationRequested)
            {
                Program.logger.Info("Saving changes in database for active users...");
                foreach (var session in _sessions.Values)
                {
                    await session.SaveProfileIfNeed();
                }
                await Task.Delay(periodicSaveDatabaseInMinutes);
            }
        }

        private async Task CloseSessionsWithTimeoutAsync()
        {
            while (!_sessionManagerTasksCTS.IsCancellationRequested)
            {
                List<ChatId> sessionsToClose = new List<ChatId>();
                foreach (var chatId in _sessions.Keys)
                {
                    var timeoutMs = _performanceManager.GetCurrentSessionTimeout() * millisecondsInHour;
                    if (IsTimeout(chatId, timeoutMs))
                    {
                        sessionsToClose.Add(chatId);
                    }
                }
                foreach (var chatId in sessionsToClose)
                {
                    await CloseSession(chatId);
                }

                await Task.Delay(10_000);
            }
        }

        private bool IsTimeout(ChatId chatId, int timeoutMs)
        {
            var session = _sessions[chatId];
            var millisecondsFromLastActivity = (int)(DateTime.UtcNow - session.lastActivityTime).TotalMilliseconds;
            return millisecondsFromLastActivity > timeoutMs;
        }

        public async Task CloseSession(ChatId chatId)
        {
            if (_sessions.TryGetValue(chatId, out var session))
            {
                _sessions.Remove(chatId);
                await session.OnCloseSession();                
            }
        }

        public async Task CloseAllSessions()
        {
            Program.logger.Info($"Closing all sessions...");
            _sessionManagerTasksCTS.Cancel();

            foreach (var chatId in _sessions.Keys)
            {
                await CloseSession(chatId);
            }
            Program.logger.Info($"All sessions closed. Profiles data saved.");
        }


    }
}
