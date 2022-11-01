using System.Net.Http;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.TelegramBot
{
    public class RequestExceptionHandler
    {
        public void HandleException(ChatId id, RequestException ex)
        {
            Program.logger.Error($"Catched exception on sending request...");
            if (ex.InnerException != null)
            {
                var innerEx = ex.InnerException;
                switch (innerEx)
                {
                    case HttpRequestException httpEx:
                        HandleHttpException(id, httpEx);
                        return;
                    case ApiRequestException apiEx:
                        HandleApiException(id, apiEx);
                        return;
                }
            }

            Program.logger.Error($"{ex.GetType().Name}: {ex.Message}");
        }

        private async void HandleHttpException(ChatId id, HttpRequestException ex)
        {
            Program.logger.Error($"HttpRequestException: {ex.Message}");

            TelegramBot.instance.Reconnect();
            var sessionManager = TelegramBot.instance.sessionManager;
            await sessionManager.CloseSession(id, onError: true);
        }

        private void HandleApiException(ChatId id, ApiRequestException ex)
        {
            Program.logger.Error($"ApiRequestException: {ex.Message}");
            // TODO
        }

    }
}
