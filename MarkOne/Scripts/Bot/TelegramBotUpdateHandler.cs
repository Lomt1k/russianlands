using System;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using FastTelegramBot.DataTypes;
using System.Collections.Generic;
using FastTelegramBot.DataTypes.Keyboards;

namespace MarkOne.Scripts.Bot;
public class TelegramBotUpdateHandler
{
    private readonly string accountIsBusyText = Emojis.ElementWarning + Localization.GetDefault("account_is_busy_message");
    private readonly ReplyKeyboardMarkup restartButton = new (Localization.GetDefault("restart_button"));

    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    public void HandleUpdates(List<Update> updates)
    {
        foreach (Update update in updates)
        {
            HandleUpdate(update);
        }
    }

    public void HandleUpdate(Update update)
    {
        //Program.logger.Debug($"Handle Update ID: {update.id} Type: {update.updateType}");
        try
        {
            User? fromUser = null;
            switch (update.UpdateType)
            {
                case UpdateType.Message: fromUser = update.Message?.From; break;
                case UpdateType.CallbackQuery: fromUser = update.CallbackQuery?.From; break;

                default:
                    Program.logger.Warn($"Unhandled Update {update.Id} (Unsupported type: {update.UpdateType})");
                    return;
            }

            if (fromUser == null)
            {
                Program.logger.Warn($"Unhandled Update {update.Id} (Update not from user)");
                return;
            }

            if (sessionManager.IsAccountUsedByFakeId(fromUser))
            {
                SendAccountIsBusyMessage(fromUser.Id);
                return;
            }

            var gameSession = sessionManager.GetOrCreateSession(fromUser);
            Task.Run(() => gameSession.HandleUpdateAsync(fromUser, update));
        }
        catch (Exception ex)
        {
            Program.logger.Error($"Exception on handle update with ID: {update.Id}\n{ex}\n");
        }
    }

    private async void SendAccountIsBusyMessage(ChatId id)
    {
        await messageSender.SendTextDialog(id, accountIsBusyText, restartButton, silent: true).FastAwait();
    }

}
