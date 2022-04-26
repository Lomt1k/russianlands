using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using System.Threading;

namespace TextGameRPG.Scripts.TelegramBot
{
    public class TelegramBotReceiving
    {
        private TelegramBot _bot;
        private CancellationTokenSource _cts;

        public bool isReceiving { get; private set; }

        public TelegramBotReceiving(TelegramBot bot)
        {
            _bot = bot;
        }

        public void StartReceiving()
        {
            _cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            _bot.client.ReceiveAsync<TelegramBotUpdateHandler>(receiverOptions, _cts.Token);
            isReceiving = true;
            //MyConsole.Log($"Listening for @{_bot.mineUser.Username}");
        }

        public void StopReceiving()
        {
            if (!isReceiving)
                return;

            _cts.Cancel();
            isReceiving = false;
            //MyConsole.Log($"Stop listening for @{_bot.mineUser.Username}");
        }


    }
}
