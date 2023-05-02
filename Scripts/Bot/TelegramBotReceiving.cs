using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TextGameRPG.Scripts.Bot;

public class TelegramBotReceiving
{
    private CancellationTokenSource _cts = new CancellationTokenSource();

    public bool isReceiving { get; private set; }

    public async Task StartReceiving()
    {
        if (isReceiving)
            return;

        var mineUser = await BotController.botClient.GetMeAsync().FastAwait();
        mineUser.CanJoinGroups = false;
        Program.SetTitle($"{mineUser.Username} [{BotController.dataPath}]");

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
        isReceiving = true;
        Program.logger.Info($"Start listening for @{mineUser.Username}");
    }

    public void StopReceiving()
    {
        if (!isReceiving)
            return;

        _cts.Cancel();
        isReceiving = false;
        Program.logger.Info($"Listening has been stopped");
    }


}
