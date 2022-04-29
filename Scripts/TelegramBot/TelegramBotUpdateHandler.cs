using System;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot
{
    using System.Threading;
    using Telegram.Bot;
    using Telegram.Bot.Exceptions;
    using Telegram.Bot.Extensions.Polling;
    using Telegram.Bot.Types;

    public class TelegramBotUpdateHandler : IUpdateHandler
    {
        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            string str = $"HandleUpdateAsync {update.Id} Type: {update.Type}";
            if (update.Message != null && update.Message.Text != null)
            {
                str += " Message: " + update.Message.Text;
            }
            Program.logger.Info(str);

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
