using MarkOne.Scripts.GameCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace MarkOne.Scripts.Bot;

public class TelegramUpdatesReceiver
{
    private static readonly UpdateType[] AllowedUpdates = new UpdateType[]
    {
        UpdateType.Message,
        UpdateType.CallbackQuery,
    };

    private CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly BotHttpListener _botHttpListener;

    public bool isReceiving { get; private set; }

    public TelegramUpdatesReceiver(BotHttpListener botHttpListener)
    {
        _botHttpListener = botHttpListener;
    }

    public async Task<bool> StartReceiving()
    {
        if (isReceiving)
        {
            return false;
        }

        _cts = new CancellationTokenSource();
        var webhookSettings = BotController.config.httpListenerSettings.telegramWebhookSettings;
        if (webhookSettings.useWebhook)
        {
            return await SetWebhook().FastAwait();
        }
        else
        {
            await DeleteWebhook().FastAwait();
            StartPollingUpdates();
            return true;
        }
    }

    private async Task<bool> SetWebhook()
    {
        var webhookUrl = _botHttpListener.externalHttpListenerPrefix + "sendUpdate";
        var botClient = BotController.botClient;
        try
        {
            var secretToken = Guid.NewGuid().ToString();
            _botHttpListener.RegisterHttpService("/sendUpdate", new TelegramUpdatesHttpSevrice(secretToken));
            await botClient.SetWebhookAsync(webhookUrl, allowedUpdates: AllowedUpdates, secretToken: secretToken);
        }
        catch (Exception ex)
        {
            Program.logger.Fatal($"Catched exception on try set telegram webhook\n{ex}");
            return false;
        }
        return true;
    }

    private async Task<bool> DeleteWebhook()
    {
        var botClient = BotController.botClient;
        try
        {
            await botClient.DeleteWebhookAsync(dropPendingUpdates: false);
        }
        catch (Exception ex)
        {
            Program.logger.Fatal($"Catched exception on try delete telegram webhook\n{ex}");
            return false;
        }
        return true;
    }

    private void StartPollingUpdates()
    {
        var receiverOptions = new ReceiverOptions
        {
            //AllowedUpdates = { }, // receive all update types
            AllowedUpdates = AllowedUpdates,
            Offset = -1 // После рестарта бота обработает только последнее сообщение, отправленное за время офлайна (оно запустит новую сессию)
        };
        BotController.botClient.ReceiveAsync<TelegramBotUpdateHandler>(receiverOptions, _cts.Token);
        isReceiving = true;
        Program.logger.Info($"Polling updates started");
    }

    public void StopReceiving()
    {
        if (!isReceiving)
            return;

        _cts.Cancel();
        isReceiving = false;
        Program.logger.Info($"Polling updates stopped");
    }


}
