using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Managers;

namespace TextGameRPG.Scripts.Bot.Sessions
{
    public class SessionManager
    {
        private const int millisecondsInMinute = 60_000;

        private static readonly PerformanceManager performanceManager = Singletones.Get<PerformanceManager>();

        private readonly int periodicSaveDatabaseInMs;
        private CancellationTokenSource _allSessionsTasksCTS;
        private Dictionary<ChatId, GameSession> _sessions = new Dictionary<ChatId, GameSession>();
        private Dictionary<long, long> _fakeIdsDict = new Dictionary<long, long>(); //cheat: allow play as another telegram user

        public int sessionsCount => _sessions.Count;
        public CancellationTokenSource allSessionsTasksCTS => _allSessionsTasksCTS;

        public SessionManager(TelegramBot telegramBot)
        {
            periodicSaveDatabaseInMs = BotConfig.instance.periodicSaveDatabaseInMinutes * millisecondsInMinute;

            _allSessionsTasksCTS = new CancellationTokenSource();
            Task.Run(() => PeriodicSaveProfilesAsync(), _allSessionsTasksCTS.Token);
            Task.Run(() => CloseSessionsWithTimeoutAsync(), _allSessionsTasksCTS.Token);
        }

        public GameSession GetOrCreateSession(User user)
        {
            if (!_sessions.TryGetValue(user.Id, out var session))
            {
                bool useFakeChatId = _fakeIdsDict.TryGetValue(user.Id, out var fakeChatId);
                session = useFakeChatId
                    ? new GameSession(user, fakeChatId)
                    : new GameSession(user);
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

        public bool IsAccountUsedByFakeId(User user)
        {
            foreach (var fakeId in _fakeIdsDict.Values)
            {
                if (fakeId == user.Id)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task PeriodicSaveProfilesAsync()
        {
            await Task.Delay(periodicSaveDatabaseInMs).FastAwait();
            while (!_allSessionsTasksCTS.IsCancellationRequested)
            {
                Program.logger.Info("Saving changes in database for active users...");
                foreach (var session in _sessions.Values)
                {
                    await session.SaveProfileIfNeed();
                }
                await Task.Delay(periodicSaveDatabaseInMs).FastAwait();
            }
        }

        private async Task CloseSessionsWithTimeoutAsync()
        {
            while (!_allSessionsTasksCTS.IsCancellationRequested)
            {
                List<ChatId> sessionsToClose = new List<ChatId>();
                var timeoutMs = BotConfig.instance.sessionTimeoutInMinutes * millisecondsInMinute;
                foreach (var chatId in _sessions.Keys)
                {
                    if (IsTimeout(chatId, timeoutMs))
                    {
                        sessionsToClose.Add(chatId);
                    }
                }
                foreach (var chatId in sessionsToClose)
                {
                    await CloseSession(chatId).FastAwait();
                }

                await Task.Delay(10_000).FastAwait();
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
                await session.OnCloseSession(onError).FastAwait();                
            }
        }

        public async Task CloseAllSessions()
        {
            Program.logger.Info($"Closing all sessions...");
            _allSessionsTasksCTS.Cancel();

            foreach (var chatId in _sessions.Keys)
            {
                await CloseSession(chatId).FastAwait();
            }
            Program.logger.Info($"All sessions closed. Profiles data saved.");
        }

        public List<GameSession> GetAllSessions()
        {
            return _sessions.Values.ToList();
        }

        // allow play as another telegram user
        public void Cheat_SetFakeId(long telegramId, long fakeId)
        {
            if (fakeId == 0 || fakeId == telegramId)
            {
                _fakeIdsDict.Remove(telegramId);
                return;
            }
            _fakeIdsDict[telegramId] = fakeId;
        }


    }
}
