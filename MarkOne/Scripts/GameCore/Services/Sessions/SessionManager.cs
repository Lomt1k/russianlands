using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.Bot;
using FastTelegramBot.DataTypes;
using FastTelegramBot.DataTypes.Keyboards;
using System.Collections.Concurrent;

namespace MarkOne.Scripts.GameCore.Sessions;

public class SessionManager : Service
{
    private const int millisecondsInMinute = 60_000;

    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private int _periodicSaveDatabaseInMs;
    private CancellationTokenSource _allSessionsTasksCTS = new CancellationTokenSource();
    private readonly ConcurrentDictionary<ChatId, GameSession> _sessions = new();
    private readonly ConcurrentDictionary<long, long> _fakeIdsDict = new(); //cheat: allow play as another telegram user

    public int sessionsCount => _sessions.Count;
    public CancellationTokenSource allSessionsTasksCTS => _allSessionsTasksCTS;

    public override Task OnBotStarted()
    {
        _periodicSaveDatabaseInMs = BotController.config.periodicSaveDatabaseInMinutes * millisecondsInMinute;
        _allSessionsTasksCTS = new CancellationTokenSource();
        Task.Run(PeriodicSaveProfilesAsync);
        Task.Run(CloseSessionsWithTimeoutAsync);
        return Task.CompletedTask;
    }

    public GameSession GetOrCreateSession(User user)
    {
        return _sessions.GetOrAdd(user.Id, (chatId) =>
        {
            var useFakeChatId = _fakeIdsDict.TryGetValue(user.Id, out var fakeChatId);
            return useFakeChatId ? new GameSession(user, fakeChatId) : new GameSession(user);
        });
    }

    public bool IsSessionExists(ChatId id)
    {
        return _sessions.ContainsKey(id);
    }

    public GameSession? GetSessionIfExists(ChatId id)
    {
        _sessions.TryGetValue(id, out var session);
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
        await Task.Delay(_periodicSaveDatabaseInMs).FastAwait();
        while (true)
        {
            try
            {
                if (sessionsCount > 0)
                {
                    Program.logger.Info("Saving changes in database for active users...");
                    foreach (var session in _sessions.Values)
                    {
                        await session.SaveProfileIfNeed();
                    }
                }
                await Task.Delay(_periodicSaveDatabaseInMs).FastAwait();
            }
            catch (Exception ex) 
            {
                Program.logger.Error(ex);
            }
        }
    }

    private async Task CloseSessionsWithTimeoutAsync()
    {
        while (true)
        {
            var sessionsToClose = new List<ChatId>();
            var timeoutMs = BotController.config.sessionTimeoutInMinutes * millisecondsInMinute;
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

    public List<GameSession> GetAllSessions()
    {
        return _sessions.Values.ToList();
    }

    public async Task CloseSession(ChatId chatId, bool onError = false, string? errorMessage = null)
    {
        if (_sessions.TryGetValue(chatId, out var session))
        {
            _sessions.TryRemove(chatId, out _);
            await session.OnCloseSession(onError, errorMessage).FastAwait();
        }
    }

    public async Task CloseAllSessions()
    {
        Program.logger.Info($"Closing all sessions...");
        _allSessionsTasksCTS.Cancel();

        var dtNow = DateTime.UtcNow;
        var lastActivePlayers = GetAllSessions()
            .Where(x => (dtNow - x.lastActivityTime).TotalMinutes < 5)
            .OrderByDescending(x => x.lastActivityTime)
            .ToArray();

        foreach (var session in GetAllSessions())
        {
            await session.OnCloseSession(false).FastAwait();
        }
        _sessions.Clear();
        Program.logger.Info($"All sessions closed. Profiles data saved.");

        await SendMaintenanceNotifications(lastActivePlayers).FastAwait();
    }

    private async Task SendMaintenanceNotifications(GameSession[] lastActivePlayers)
    {
        if (!BotController.config.performanceSettings.sendMaintenanceNotificationsOnStop)
            return;

        Program.logger.Info("Sending notifications for active players...");

        var secondsLimit = BotController.config.performanceSettings.secondsLimitForSendMaintenance;
        var playersCountLimit = secondsLimit * BotController.config.sendingLimits.sendMessagePerSecondLimit;
        if (playersCountLimit < 1 || playersCountLimit > lastActivePlayers.Length)
        {
            playersCountLimit = lastActivePlayers.Length;
        }

        var sendMessageTasks = new List<Task>();
        for (var i = 0; i < playersCountLimit; i++)
        {
            var session = lastActivePlayers[i];
            var text = Emojis.ElementWarning + Localization.Get(session, "maintenance_message");
            var button = new ReplyKeyboardMarkup(Localization.GetDefault("restart_button"));
            var task = messageSender.SendTextDialog(session.chatId, text, button, silent: true);
            sendMessageTasks.Add(task);
        }

        await Task.WhenAll(sendMessageTasks).FastAwait();
        Program.logger.Info("Notifications sending completed");
    }

    // allow play as another telegram user
    public void Cheat_SetFakeId(long telegramId, long fakeId)
    {
        if (fakeId == 0 || fakeId == telegramId)
        {
            _fakeIdsDict.TryRemove(telegramId, out _);
            return;
        }
        _fakeIdsDict[telegramId] = fakeId;
    }


}
