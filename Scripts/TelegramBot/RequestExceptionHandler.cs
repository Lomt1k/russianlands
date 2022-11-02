using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.TelegramBot
{
    public class RequestExceptionHandler
    {
        public async Task HandleException(ChatId id, RequestException ex)
        {
            var user = TelegramBot.instance.sessionManager.GetSessionIfExists(id)?.actualUser;
            Program.logger.Error($"Catched exception on sending request to " + (user != null ? user.ToString() : id.ToString()));

            var hanldingEx = ex.InnerException ?? ex;
            switch (hanldingEx)
            {
                case HttpRequestException httpEx:
                    await HandleHttpException(id, httpEx);
                    return;
                case ApiRequestException apiEx:
                    await HandleApiException(id, apiEx);
                    return;
            }

            Program.logger.Error($"{hanldingEx.GetType().Name}: {hanldingEx.Message}");
        }

        private async Task HandleHttpException(ChatId id, HttpRequestException ex)
        {
            Program.logger.Error($"HttpRequestException: {ex.Message}");

            TelegramBot.instance.Reconnect();
            var sessionManager = TelegramBot.instance.sessionManager;
            await sessionManager.CloseSession(id, onError: true);
        }

        private async Task HandleApiException(ChatId id, ApiRequestException ex)
        {
            Program.logger.Error($"ApiRequestException: {ex.Message}");
            var sessionManager = TelegramBot.instance.sessionManager;
            await sessionManager.CloseSession(id, onError: true);
        }

    }
}
