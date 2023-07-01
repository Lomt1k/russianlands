using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.CallbackData;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Profiles;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Dialogs;
using MarkOne.Scripts.GameCore.Commands;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using FastTelegramBot.DataTypes;

namespace MarkOne.Scripts.GameCore.Sessions;

public class GameSession
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly SessionExceptionHandler sessionExceptionHandler = ServiceLocator.Get<SessionExceptionHandler>();
    private static readonly PerformanceManager performanceManager = ServiceLocator.Get<PerformanceManager>();
    private static readonly BattleManager battleManager = ServiceLocator.Get<BattleManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private bool _isHandlingUpdate;
    private readonly CancellationTokenSource _sessionTasksCTS = new CancellationTokenSource();

    public ChatId chatId { get; }
    public ChatId? fakeChatId { get; }
    public DateTime startTime { get; }
    public DateTime lastActivityTime { get; private set; }
    public User actualUser { get; private set; }
    public Profile profile { get; private set; }
    public Player player { get; private set; }
    public LanguageCode language => profile?.data.language ?? BotController.config.defaultLanguageCode;
    public DialogBase? currentDialog { get; private set; }
    public TooltipController tooltipController { get; } = new TooltipController();
    public CancellationToken cancellationToken { get; }
    public bool isAdmin => profile.data.adminStatus >= AdminStatus.Admin;

    public GameSession(User user, ChatId? _fakeChatId = null)
    {
        chatId = new ChatId(user.Id);
        fakeChatId = _fakeChatId;
        var now = DateTime.UtcNow;
        startTime = now;
        lastActivityTime = now;
        cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_sessionTasksCTS.Token, sessionManager.allSessionsTasksCTS.Token).Token;

        Program.logger.Info($"Started a new session for {user}"
            + (fakeChatId?.Id != null ? $" with fakeId: {fakeChatId}" : string.Empty));
        Program.logger.Info($"Sessions Count: {sessionManager.sessionsCount + 1}");
    }

    public void SetupActiveDialog(DialogBase dialog)
    {
        currentDialog?.OnClose();
        currentDialog = dialog;
    }

    public async Task HandleUpdateAsync(User refreshedUser, Update update)
    {
        if (_isHandlingUpdate)
        {
            if (update.CallbackQuery is not null)
            {
                await messageSender.AnswerQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken).FastAwait();
            }
            return;
        }

        _isHandlingUpdate = true;
        try
        {
            // Делаем паузу на обработку апдейта в зависимости от нагрузки на процессор
            await Task.Delay(performanceManager.currentResponceDelay, cancellationToken).FastAwait();
            if (cancellationToken.IsCancellationRequested)
                return;

            var previousActivity = lastActivityTime;
            lastActivityTime = DateTime.UtcNow;
            actualUser = refreshedUser;
            if (profile == null)
            {
                await OnStartNewSession(update).FastAwait();
                _isHandlingUpdate = false;
                return;
            }

            // Регистрируем игровую активность для статистики
            var secondsBeforeActivities = (lastActivityTime - previousActivity).Seconds;
            if (secondsBeforeActivities < 120)
            {
                profile.dailyData.activityInSeconds += secondsBeforeActivities;
            }

            // Обрабатываем апдейт
            switch (update.UpdateType)
            {
                case UpdateType.Message:
                    await HandleMessageAsync(update.Message).FastAwait();
                    break;
                case UpdateType.CallbackQuery:
                    await HandleQueryAsync(update.CallbackQuery).FastAwait();
                    break;
            }
        }
        catch (Exception ex)
        {
            await sessionExceptionHandler.HandleException(refreshedUser, ex);
        }
        _isHandlingUpdate = false;
    }


    public async Task HandleMessageAsync(Message message)
    {
        if (BotController.config.logSettings.logUpdates && message.Text != null)
        {
            Program.logger.InfoFormat("UPDATE :: {0}: {1}", actualUser, message.Text);
        }

        if (message.Text != null && message.Text.StartsWith('/'))
        {
            await CommandHandler.HandleCommand(this, message.Text).FastAwait();
            return;
        }
        await currentDialog.HandleMessage(message).FastAwait();
    }

    public async Task HandleQueryAsync(CallbackQuery query)
    {
        if (query == null || query.Data == null)
            return;

        var callbackData = JsonConvert.DeserializeObject<CallbackDataBase>(query.Data);
        switch (callbackData)
        {
            case DialogPanelButtonCallbackData dialogPanelButtonCallback:
                if (dialogPanelButtonCallback.sessionTime != startTime.Ticks)
                {
                    return;
                }
                if (currentDialog != null && currentDialog is DialogWithPanel dialogWithPanel)
                {
                    await dialogWithPanel.HandleCallbackQuery(query.Id, dialogPanelButtonCallback).FastAwait();
                }
                return;

            case BattleTooltipCallbackData battleTooltipCallback:
                if (player == null)
                    return;
                var currentBattle = battleManager.GetCurrentBattle(player);
                if (currentBattle == null)
                    return;
                await currentBattle.HandleBattleTooltipCallback(player, query.Id, battleTooltipCallback).FastAwait();
                return;
        }
    }

    private async Task OnStartNewSession(Update update)
    {
        profile = await Profile.Load(this).FastAwait();
        player = new Player(this);
        await QuestManager.HandleNewSession(this, update).FastAwait();
    }

    public async Task OnCloseSession(bool onError)
    {
        await SaveProfileIfNeed().FastAwait();
        _sessionTasksCTS.Cancel();
        Program.logger.Info($"Session closed for {actualUser}" + (onError ? " [ON ERROR]" : string.Empty));

        if (onError)
        {
            battleManager.OnSessionClosedWithError(player);
        }
    }

    public async Task SaveProfileIfNeed()
    {
        if (profile != null)
        {
            await profile.SaveProfileIfNeed(lastActivityTime).FastAwait();
        }
    }

}
