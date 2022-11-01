using Telegram.Bot;
using Telegram.Bot.Polling;
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
            if (isReceiving)
                return;

            _cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
                Offset = -1 // После рестарта бота обработает только последнее сообщение, отправленное за время офлайна (оно запустит новую сессию)
            };
            _bot.client.ReceiveAsync<TelegramBotUpdateHandler>(receiverOptions, _cts.Token);
            isReceiving = true;
            Program.logger.Info($"Start listening for @{_bot.mineUser.Username}");
        }

        public void StopReceiving()
        {
            if (!isReceiving)
                return;

            _cts.Cancel();
            isReceiving = false;
            Program.logger.Info($"Stop listening for @{_bot.mineUser.Username}");
        }


    }
}
