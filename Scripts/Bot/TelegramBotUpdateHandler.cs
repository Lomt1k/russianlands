﻿using System;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.Bot
{
    using System.Threading;
    using Telegram.Bot;
    using Telegram.Bot.Exceptions;
    using Telegram.Bot.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;
    using TextGameRPG.Scripts.GameCore.Managers;
    using TextGameRPG.Scripts.Bot.Sessions;
    using TextGameRPG.Scripts.GameCore.Localizations;

    public class TelegramBotUpdateHandler : IUpdateHandler
    {
        private readonly string accountIsBusyText = Emojis.ElementWarning + Localization.GetDefault("account_is_busy_message");
        private readonly ReplyKeyboardMarkup restartButton = new ReplyKeyboardMarkup(Localization.GetDefault("restart_button"));

        private static readonly SessionManager sessionManager = Singletones.Get<SessionManager>();
        private static readonly PerformanceManager performanceManager = Singletones.Get<PerformanceManager>();
        private static readonly MessageSender messageSender = Singletones.Get<MessageSender>();

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                gameSession.HandleUpdateAsync(fromUser, update);
            }
            catch (Exception ex)
            {
                Program.logger.Error($"Exception on handle update with ID: {update.Id}\n{ex}\n");
                return;
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
}
