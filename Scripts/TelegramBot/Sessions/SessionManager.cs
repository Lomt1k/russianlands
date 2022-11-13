using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly int periodicSaveDatabaseInMs;

        private PerformanceManager _performanceManager;
        private CancellationTokenSource _allSessionsTasksCTS;
        private Dictionary<ChatId, GameSession> _sessions = new Dictionary<ChatId, GameSession>();

        public int sessionsCount => _sessions.Count;
        public CancellationTokenSource allSessionsTasksCTS => _allSessionsTasksCTS;

        public SessionManager(TelegramBot telegramBot)
        {
            _performanceManager = GlobalManagers.performanceManager;
            periodicSaveDatabaseInMs = telegramBot.config.periodicSaveDatabaseInMinutes * millisecondsInMinute;

            _allSessionsTasksCTS = new CancellationTokenSource();
            Task.Run(() => PeriodicSaveProfilesAsync(), _allSessionsTasksCTS.Token);
            Task.Run(() => CloseSessionsWithTimeoutAsync(), _allSessionsTasksCTS.Token);
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

        public bool IsSessionExists(ChatId id)
        {
            return _sessions.ContainsKey(id);
        }

        public GameSession? GetSessionIfExists(ChatId id)
        {
            _sessions.TryGetValue(id, out GameSession? session);
            return session;
        }

        public bool HasActiveSession(User user)
        {
            return _sessions.ContainsKey(user.Id);
        }

        private async Task PeriodicSaveProfilesAsync()
        {
            await Task.Delay(periodicSaveDatabaseInMs).ConfigureAwait(false);
            while (!_allSessionsTasksCTS.IsCancellationRequested)
            {
                Program.logger.Info("Saving changes in database for active users...");
                foreach (var session in _sessions.Values)
                {
                    await session.SaveProfileIfNeed();
                }
                await Task.Delay(periodicSaveDatabaseInMs).ConfigureAwait(false);
            }
        }

        private async Task CloseSessionsWithTimeoutAsync()
        {
            while (!_allSessionsTasksCTS.IsCancellationRequested)
            {
                List<ChatId> sessionsToClose = new List<ChatId>();
                foreach (var chatId in _sessions.Keys)
                {
                    var timeoutMs = _performanceManager.GetCurrentSessionTimeout() * millisecondsInMinute;
                    if (IsTimeout(chatId, timeoutMs))
                    {
                        sessionsToClose.Add(chatId);
                    }
                }
                foreach (var chatId in sessionsToClose)
                {
                    await CloseSession(chatId).ConfigureAwait(false);
                }

                await Task.Delay(10_000).ConfigureAwait(false);
            }
        }

        private bool IsTimeout(ChatId chatId, int timeoutMs)
        {
            var session = _sessions[chatId];
            var millisecondsFromLastActivity = (int)(DateTime.UtcNow - session.lastActivityTime).TotalMilliseconds;
            return millisecondsFromLastActivity > timeoutMs;
        }

        public async Task CloseSession(ChatId chatId, bool onError = false)
        {
            if (_sessions.TryGetValue(chatId, out var session))
            {
                _sessions.Remove(chatId);
                await session.OnCloseSession(onError).ConfigureAwait(false);                
            }
        }

        public async Task CloseAllSessions()
        {
            Program.logger.Info($"Closing all sessions...");
            _allSessionsTasksCTS.Cancel();

            foreach (var chatId in _sessions.Keys)
            {
                await CloseSession(chatId).ConfigureAwait(false);
            }
            Program.logger.Info($"All sessions closed. Profiles data saved.");
        }

        public List<GameSession> GetAllSessions()
        {
            return _sessions.Values.ToList();
        }


    }
}
