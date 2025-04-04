﻿using Newtonsoft.Json;
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
using static System.Collections.Specialized.BitVector32;

namespace MarkOne.Scripts.GameCore.Sessions;

public class GameSession
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly SessionExceptionHandler sessionExceptionHandler = ServiceLocator.Get<SessionExceptionHandler>();
    private static readonly PerformanceManager performanceManager = ServiceLocator.Get<PerformanceManager>();
    private static readonly BattleManager battleManager = ServiceLocator.Get<BattleManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private bool _isHandlingUpdate;
    private readonly CancellationTokenSource _sessionTasksCTS = new();
    private CancellationTokenSource _handleUpdateTimeoutCTS = new();

    public ChatId chatId { get; }
    public ChatId? fakeChatId { get; }
    public DateTime startTime { get; }
    public DateTime lastActivityTime { get; private set; }
    public DateTime lastStartOfferTime { get; set; }
    public User actualUser { get; private set; }
    public Profile profile { get; private set; }
    public Player player { get; private set; }
    public LanguageCode language => profile?.data.language ?? BotController.config.defaultLanguageCode;
    public DialogBase? currentDialog { get; private set; }
    public TooltipController tooltipController { get; } = new TooltipController();
    public CancellationToken cancellationToken { get; }
    public bool isAdmin => profile.data.adminStatus >= AdminStatus.Admin;
    public bool isTutorialCompleted => player.buildings.GetBuildingLevel(Buildings.BuildingId.TownHall) >= 2;

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

            _handleUpdateTimeoutCTS = new();
            Task.Run(() => HandleUpdateTimeout(_handleUpdateTimeoutCTS.Token));

            if (profile == null)
            {
                await OnStartNewSession(update).FastAwait();
                _handleUpdateTimeoutCTS.Cancel();
                _isHandlingUpdate = false;
                return;
            }

            // Регистрируем игровую активность для статистики
            var secondsBeforeActivity = (lastActivityTime - previousActivity).Seconds;
            if (secondsBeforeActivity < 180)
            {
                profile.dailyData.activityInSeconds += secondsBeforeActivity;
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

        _handleUpdateTimeoutCTS.Cancel();
        _isHandlingUpdate = false;
    }

    /* Есть предположение, что иногда виснет _isHandlingUpdate == true, из-за чего апдейты юзера перестают обрабатываться.
     * Этот таймаут должен починить игру не дожидаясь обычного таймаута сессии
     */
    private async Task HandleUpdateTimeout(CancellationToken cancellationToken)
    {
        await Task.Delay(BotController.config.performanceSettings.handleUpdateTimeoutInSeconds * 1000, CancellationToken.None).FastAwait();
        if (cancellationToken.IsCancellationRequested || this.cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Program.logger.Error($"Catched handle update timeout for {actualUser}");
        await sessionManager.CloseSession(actualUser.Id, true, "HANDLE UPDATE TIMEOUT").FastAwait();
        var text = Localization.Get(language, "handle_update_timeout_error");
        Task.Run(() => messageSender.SendErrorMessage(actualUser.Id, text));
    }

    private async Task HandleMessageAsync(Message message)
    {
        if (BotController.config.logSettings.logUpdates && message.Text != null)
        {
            Program.logger.Info($"UPDATE :: {actualUser}: {message.Text}");
        }

        if (message.Text != null && message.Text.StartsWith('/'))
        {
            await CommandHandler.HandleCommand(this, message.Text).FastAwait();
            return;
        }
        if (currentDialog is not null)
        {
            await currentDialog.HandleMessage(message).FastAwait();
        }
    }

    private async Task HandleQueryAsync(CallbackQuery query)
    {
        if (query == null || query.Data == null)
            return;

        var callbackData = JsonConvert.DeserializeObject<CallbackDataBase>(query.Data);
        switch (callbackData)
        {
            case DialogPanelButtonCallbackData dialogPanelButtonCallback:
                if (currentDialog != null && currentDialog is DialogWithPanel dialogWithPanel)
                {
                    await dialogWithPanel.HandleCallbackQuery(query.Id, dialogPanelButtonCallback).FastAwait();
                    return;
                }
                await AnswerInvalidQueryAsync(query.Id).FastAwait();
                return;

            case BattleTooltipCallbackData battleTooltipCallback:
                if (player == null)
                {
                    await AnswerInvalidQueryAsync(query.Id).FastAwait();
                    return;
                }
                var currentBattle = battleManager.GetCurrentBattle(player);
                if (currentBattle == null)
                {
                    await AnswerInvalidQueryAsync(query.Id).FastAwait();
                    return;
                }
                await currentBattle.HandleBattleTooltipCallback(player, query.Id, battleTooltipCallback).FastAwait();
                return;
        }
    }

    private async Task AnswerInvalidQueryAsync(string queryId)
    {
        await messageSender.AnswerQuery(queryId, Localization.Get(this, "invalid_query_answer"), cancellationToken).FastAwait();
    }

    private async Task OnStartNewSession(Update update)
    {
        var messageText = update?.Message?.Text ?? string.Empty;
        profile = await Profile.Load(this, messageText).FastAwait();
        player = new Player(this);
        await QuestManager.HandleNewSession(this, update).FastAwait();
    }

    public async Task OnCloseSession(bool onError, string? errorMessage = null)
    {
        try
        {
            await SaveProfileIfNeed().FastAwait();
            _sessionTasksCTS.Cancel();
            if (onError)
            {
                var message = errorMessage ?? "ON ERROR";
                Program.logger.Info($"Session closed for {actualUser} [{message}]");
            }
            else
            {
                Program.logger.Info($"Session closed for {actualUser}");
            }
        }
        catch (Exception ex)
        {
            Program.logger.Error(ex);
        }
    }

    public async Task SaveProfileIfNeed()
    {
        try
        {
            if (profile != null)
            {
                await profile.SaveProfileIfNeed(lastActivityTime).FastAwait();
            }
        }
        catch (Exception ex)
        {
            Program.logger.Error(ex);
        }
        
    }

}
