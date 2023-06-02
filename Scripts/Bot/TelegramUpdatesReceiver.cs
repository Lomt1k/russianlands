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
            return await StartWithWebhook().FastAwait();
        }
        else
        {
            StartPollingUpdates();
            return true;
        }
    }

    private async Task<bool> StartWithWebhook()
    {
        var webhookUrl = _botHttpListener.externalHttpListenerPrefix + "sendUpdate";
        var botClient = BotController.botClient;
        try
        {
            _botHttpListener.RegisterHttpService("/sendUpdate", new TelegramUpdatesHttpSevrice());
            await botClient.SetWebhookAsync(webhookUrl);
        }
        catch (Exception ex)
        {
            Program.logger.Fatal($"Catched exception on try set telegram webhook\n{ex}");
            return false;
        }
        return true;
    }

    private void StartPollingUpdates()
    {
        var receiverOptions = new ReceiverOptions
        {
            //AllowedUpdates = { }, // receive all update types
            AllowedUpdates = new UpdateType[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery,
            },
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
