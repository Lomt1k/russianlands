using System;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot
{
    using System.Threading;
    using Telegram.Bot;
    using Telegram.Bot.Exceptions;
    using Telegram.Bot.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;
    using TextGameRPG.Scripts.TelegramBot.Managers;
    using TextGameRPG.Scripts.TelegramBot.Sessions;

    public class TelegramBotUpdateHandler : IUpdateHandler
    {
        private readonly string serverIsBusyText = $"{Emojis.elements[Element.Warning]} Server is busy. Please try later...";
        private readonly ReplyKeyboardMarkup serverIsBusyKeyboard = new ReplyKeyboardMarkup("Restart");

        private SessionManager _sessionManager;
        private PerformanceManager _performanceManager;
        private MessageSender _messageSender;

        public TelegramBotUpdateHandler()
        {
            _sessionManager = TelegramBot.instance.sessionManager;
            _performanceManager = GlobalManagers.performanceManager;
            _messageSender = TelegramBot.instance.messageSender;
        }

        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //Program.logger.Debug($"Handle Update ID: {update.Id} Type: {update.Type}");
            try
            {
                User? fromUser = null;
                switch (update.Type)
                {
                    case UpdateType.Message: fromUser = update.Message?.From; break;
                    case UpdateType.CallbackQuery: fromUser = update.CallbackQuery?.From; break;

                    default:
                        Program.logger.Warn($"Unhandled Update {update.Id} (unsupported type: {update.Type})");
                        return Task.CompletedTask;
                }

                if (fromUser == null)
                {
                    Program.logger.Warn($"Unhandled Update {update.Id} (Update not from user)");
                    return Task.CompletedTask;
                }

                bool serverIsBusy = _performanceManager.currentState == PerformanceState.Busy;
                var gameSession = serverIsBusy
                    ? _sessionManager.GetSessionIfExists(fromUser.Id)
                    : _sessionManager.GetOrCreateSession(fromUser);

                if (gameSession != null)
                {
                    gameSession.HandleUpdateAsync(fromUser, update);
                }
                else
                {
                    _messageSender.SendTextDialog(fromUser.Id, serverIsBusyText, serverIsBusyKeyboard, silent: true);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Program.logger.Error($"Exception on handle update with ID: {update.Id}\n{ex}\n");
                return Task.CompletedTask;
            }            
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
}
