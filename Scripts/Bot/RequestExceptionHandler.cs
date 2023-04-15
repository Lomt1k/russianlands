using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.Bot
{
    public class RequestExceptionHandler
    {
        private static readonly SessionManager sessionManager = Services.Get<SessionManager>();

        public async Task HandleException(ChatId id, RequestException ex)
        {
            var user = sessionManager.GetSessionIfExists(id)?.actualUser;
            Program.logger.Error($"Catched exception on sending request to " + (user != null ? user.ToString() : id.ToString()));

            var hanldingEx = ex.InnerException ?? ex;
            switch (hanldingEx)
            {
                case HttpRequestException httpEx:
                    await HandleHttpException(id, httpEx).FastAwait();
                    return;
                case ApiRequestException apiEx:
                    await HandleApiException(id, apiEx).FastAwait();
                    return;
                default:
                    await HandleUnknownException(id, hanldingEx).FastAwait();
                    return;
            }
        }

        private async Task HandleHttpException(ChatId id, HttpRequestException ex)
        {
            Program.logger.Error($"HttpRequestException: {ex.Message}");

            BotController.Reconnect();
            await sessionManager.CloseSession(id, onError: true).FastAwait();
        }

        private async Task HandleApiException(ChatId id, ApiRequestException ex)
        {
            Program.logger.Error($"ApiRequestException: {ex.Message}");
            await sessionManager.CloseSession(id, onError: true).FastAwait();
        }

        private async Task HandleUnknownException(ChatId id, System.Exception ex)
        {
            Program.logger.Error("Unkwown Exception: " + ex);
            await sessionManager.CloseSession(id, onError: true).FastAwait();
        }

    }
}
