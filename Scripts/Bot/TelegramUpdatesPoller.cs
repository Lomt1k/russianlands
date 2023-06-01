using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace MarkOne.Scripts.Bot;

public class TelegramUpdatesPoller
{
    private CancellationTokenSource _cts = new CancellationTokenSource();

    public bool isPolling { get; private set; }

    public bool StartPolling()
    {
        if (isPolling)
            return false;

        _cts = new CancellationTokenSource();
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
        isPolling = true;
        Program.logger.Info($"Polling updates started");
        return true;
    }

    public void StopPolling()
    {
        if (!isPolling)
            return;

        _cts.Cancel();
        isPolling = false;
        Program.logger.Info($"Polling updates stopped");
    }


}
