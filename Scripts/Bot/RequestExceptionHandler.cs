using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.Bot
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
                    await HandleHttpException(id, httpEx).ConfigureAwait(false);
                    return;
                case ApiRequestException apiEx:
                    await HandleApiException(id, apiEx).ConfigureAwait(false);
                    return;
                default:
                    await HandleUnknownException(id, hanldingEx).ConfigureAwait(false);
                    return;
            }
        }

        private async Task HandleHttpException(ChatId id, HttpRequestException ex)
        {
            Program.logger.Error($"HttpRequestException: {ex.Message}");

            TelegramBot.instance.Reconnect();
            var sessionManager = TelegramBot.instance.sessionManager;
            await sessionManager.CloseSession(id, onError: true).ConfigureAwait(false);
        }

        private async Task HandleApiException(ChatId id, ApiRequestException ex)
        {
            Program.logger.Error($"ApiRequestException: {ex.Message}");
            var sessionManager = TelegramBot.instance.sessionManager;
            await sessionManager.CloseSession(id, onError: true).ConfigureAwait(false);
        }

        private async Task HandleUnknownException(ChatId id, System.Exception ex)
        {
            Program.logger.Error("Unkwown Exception: " + ex);
            var sessionManager = TelegramBot.instance.sessionManager;
            await sessionManager.CloseSession(id, onError: true).ConfigureAwait(false);
        }

    }
}
