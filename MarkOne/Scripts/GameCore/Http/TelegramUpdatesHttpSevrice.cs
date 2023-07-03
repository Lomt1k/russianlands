using FastTelegramBot;
using MarkOne.Scripts.Bot;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http;
public class TelegramUpdatesHttpSevrice : IHttpService
{
    private readonly string _secretToken;
    private readonly TelegramBotUpdateHandler _updateHandler = new();

    public TelegramUpdatesHttpSevrice(string secretToken)
    {
        _secretToken = secretToken;
    }

    public Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var update = TelegramBotClient.GetUpdateFromWebhookListener(request, _secretToken);
        if (update is null)
        {
            response.StatusCode = 404;
            response.Close();
            return Task.CompletedTask;
        }

        response.Close();
        _updateHandler.HandleUpdate(update);
        return Task.CompletedTask;
    }


}
