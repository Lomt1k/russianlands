using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.Bot;
public class TelegramBotUpdateHandler : IUpdateHandler
{
    private readonly string accountIsBusyText = Emojis.ElementWarning + Localization.GetDefault("account_is_busy_message");
    private readonly ReplyKeyboardMarkup restartButton = new ReplyKeyboardMarkup(Localization.GetDefault("restart_button"));

    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    // Telegram updates from polling
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //Program.logger.Debug($"Handle Update ID: {update.Id} Type: {update.Type}");
        var simpleUpdate = update.ToSimple();
        if (simpleUpdate is not null)
        {
            HandleSimpleUpdate(simpleUpdate);
        }
        return Task.CompletedTask;
    }

    public void HandleSimpleUpdate(SimpleUpdate update)
    {
        //Program.logger.Debug($"Handle Update ID: {update.id} Type: {update.updateType}");
        try
        {
            SimpleUser? fromUser = null;
            switch (update.updateType)
            {
                case UpdateType.Message: fromUser = update.message?.from; break;
                case UpdateType.CallbackQuery: fromUser = update.callbackQuery?.from; break;

                default:
                    Program.logger.Warn($"Unhandled Update {update.id} (Unsupported type: {update.updateType})");
                    return;
            }

            if (fromUser == null)
            {
                Program.logger.Warn($"Unhandled Update {update.id} (Update not from user)");
                return;
            }

            if (sessionManager.IsAccountUsedByFakeId(fromUser))
            {
                SendAccountIsBusyMessage(fromUser.id);
                return;
            }

            var gameSession = sessionManager.GetOrCreateSession(fromUser);
            Task.Run(() => gameSession.HandleUpdateAsync(fromUser, update));
        }
        catch (Exception ex)
        {
            Program.logger.Error($"Exception on handle update with ID: {update.id}\n{ex}\n");
        }
    }

    private async void SendAccountIsBusyMessage(ChatId id)
    {
        await messageSender.SendTextDialog(id, accountIsBusyText, restartButton, silent: true).FastAwait();
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ApiRequestException apiRequestException:
                Program.logger.Error($"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}");
                break;
        }

        return Task.CompletedTask;
    }
}
