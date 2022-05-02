using System;
using System.Collections.Generic;
using System.Threading;
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
        private CancellationTokenSource _periodicSaveCTS;
        private CancellationTokenSource _closeTimeoutSessionsCTS;
        private Dictionary<ChatId, GameSession> _sessions = new Dictionary<ChatId, GameSession>();

        public SessionManager(TelegramBot telegramBot)
        {
            _telegramBot = telegramBot;
            sessionTimeoutInMilliseconds = telegramBot.config.sessionTimeoutInHours * millisecondsInHour;
            periodicSaveDatabaseInMinutes = telegramBot.config.periodicSaveDatabaseInMinutes * millisecondsInMinute;

            _periodicSaveCTS = new CancellationTokenSource();
            _closeTimeoutSessionsCTS = new CancellationTokenSource();
            Task.Run(() => PeriodicSaveProfilesAsync(), _periodicSaveCTS.Token);
            Task.Run(() => CloseSessionsWithTimeoutAsync(), _closeTimeoutSessionsCTS.Token);
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

        public GameSession? GetSessionIfExists(ChatId chatId)
        {
            _sessions.TryGetValue(chatId, out GameSession? session);
            return session;
        }

        public bool HasActiveSession(ChatId chatId)
        {
            return _sessions.ContainsKey(chatId);
        }

        private async Task PeriodicSaveProfilesAsync()
        {
            await Task.Delay(periodicSaveDatabaseInMinutes);
            while (!_periodicSaveCTS.IsCancellationRequested)
            {
                Program.logger.Info("Saving changes in database for active users...");
                foreach (var session in _sessions.Values)
                {
                    session.SaveProfileIfNeed();
                }
                await Task.Delay(periodicSaveDatabaseInMinutes);
            }
        }

        private async Task CloseSessionsWithTimeoutAsync()
        {
            while (!_closeTimeoutSessionsCTS.IsCancellationRequested)
            {
                List<ChatId> sessionsToClose = new List<ChatId>();
                foreach (var chatId in _sessions.Keys)
                {
                    if (IsTimeout(chatId))
                    {
                        sessionsToClose.Add(chatId);
                    }
                }
                foreach (var chatId in sessionsToClose)
                {
                    CloseSession(chatId);
                }

                await Task.Delay(10_000);
            }
        }

        private bool IsTimeout(ChatId chatId)
        {
            var session = _sessions[chatId];
            var millisecondsFromLastActivity = (int)(DateTime.UtcNow - session.lastActivityTime).TotalMilliseconds;
            return millisecondsFromLastActivity > sessionTimeoutInMilliseconds;
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
            _periodicSaveCTS.Cancel();
            _closeTimeoutSessionsCTS.Cancel();
            foreach (var chatId in _sessions.Keys)
            {
                await CloseSession(chatId);
            }
            Program.logger.Info($"All sessions closed. Profiles data saved.");
        }


    }
}
