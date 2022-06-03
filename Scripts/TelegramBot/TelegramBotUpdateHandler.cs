using System;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot
{
    using System.Threading;
    using Telegram.Bot;
    using Telegram.Bot.Exceptions;
    using Telegram.Bot.Extensions.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using TextGameRPG.Scripts.TelegramBot.Sessions;

    public class TelegramBotUpdateHandler : IUpdateHandler
    {
        private SessionManager _sessionManager;

        public TelegramBotUpdateHandler()
        {
            _sessionManager = TelegramBot.instance.sessionManager;
        }

        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //Program.logger.Debug($"Handle Update ID: {update.Id} Type: {update.Type}");

            User? fromUser = null;
            switch (update.Type)
            {
                case UpdateType.Message: fromUser = update.Message?.From; break;
                case UpdateType.CallbackQuery: fromUser = update.CallbackQuery?.From; break;

                default:
                    Program.logger.Warn($"Unhandled Update {update.Id} (unsupported type)");
                    return Task.CompletedTask;
            }

            if (fromUser == null)
            {
                Program.logger.Warn($"Unhandled Update {update.Id} (User is NULL)");
                return Task.CompletedTask;
            }

            var gameSession = _sessionManager.GetOrCreateSession(fromUser);
            gameSession.HandleUpdateAsync(fromUser, update);

            return Task.CompletedTask;
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
