using System.Net.Http;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using FastTelegramBot;
using FastTelegramBot.DataTypes;
using System;

namespace MarkOne.Scripts.Bot;

public class SessionExceptionHandler : Service
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    public async Task HandleException(User user, Exception ex)
    {
        var hanldingEx = ex.InnerException ?? ex;
        switch (hanldingEx)
        {
            case TaskCanceledException:
                return;
            case HttpRequestException httpEx:
                await HandleHttpException(user, httpEx).FastAwait();
                return;
            case TelegramBotException apiEx:
                await HandleTelegramBotException(user, apiEx).FastAwait();
                return;
            default:
                await HandleUnknownException(user, hanldingEx).FastAwait();
                return;
        }
    }

    private async Task HandleHttpException(User user, HttpRequestException ex)
    {
        Program.logger.Error($"Catched exception in session for user {user}");
        Program.logger.Error($"HttpRequestException: {ex.Message}");

        BotController.Reconnect();
        await sessionManager.CloseSession(user.Id, onError: true).FastAwait();
    }

    private async Task HandleTelegramBotException(User user, TelegramBotException ex)
    {
        if (ex.ErrorCode == 403)
        {
            await sessionManager.CloseSession(user.Id, onError: true, "BLOCKED BY USER").FastAwait();
            return;
        }
        Program.logger.Error($"Catched exception in session for user {user}");
        Program.logger.Error($"TelegramBotException: {ex.Message}");
        await sessionManager.CloseSession(user.Id, onError: true).FastAwait();
    }

    private async Task HandleUnknownException(User user, Exception ex)
    {
        Program.logger.Error($"Catched exception in session for user {user}");
        Program.logger.Error(ex);
        await messageSender.SendErrorMessage(user.Id, $"{ex.GetType()}: {ex.Message}").FastAwait();
        await sessionManager.CloseSession(user.Id, onError: true).FastAwait();
    }

}
