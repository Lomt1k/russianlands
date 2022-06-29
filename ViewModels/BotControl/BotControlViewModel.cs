using System;
using TextGameRPG.Scripts.TelegramBot;
using ReactiveUI;
using TextGameRPG.Scripts.Utils;
using System.Reactive;
using System.Threading.Tasks;

namespace TextGameRPG.ViewModels.BotControl
{
    public class BotControlViewModel : ViewModelBase
    {
        private string _consoleOutput = string.Empty;
        private TelegramBot _bot;
        private bool _isBotListening;
        private bool _canStartListening = true;
        private bool _canStopListening;

        public string consoleOutput
        {
            get => _consoleOutput;
            set => this.RaiseAndSetIfChanged(ref _consoleOutput, value);
        }

        public bool isBotListening
        {
            get => _isBotListening;
            set => this.RaiseAndSetIfChanged(ref _isBotListening, value);
        }

        public bool canStartListening
        {
            get => _canStartListening;
            set => this.RaiseAndSetIfChanged(ref _canStartListening, value);
        }

        public bool canStopListening
        {
            get => _canStopListening;
            set => this.RaiseAndSetIfChanged(ref _canStopListening, value);
        }

        public ReactiveCommand<Unit, Task> startListening { get; }
        public ReactiveCommand<Unit, Task> stopListening { get; }

        public BotControlViewModel(TelegramBot bot)
        {
            RedirectConsoleOutput();
            Program.logger.Info($"Selected bot data: {bot.dataPath}");
            _bot = bot;
            _bot.Init();

            startListening = ReactiveCommand.Create(StartBotListening);
            stopListening = ReactiveCommand.Create(StopBotListening);
        }

        private void RedirectConsoleOutput()
        {
            var textWriter = new TextWriterToString((output) => consoleOutput = output);
            Console.SetOut(textWriter);
        }

        private async Task StartBotListening()
        {
            canStartListening = false;

            bool success = await _bot.StartListeningAsync();
            isBotListening = success;

            canStartListening = !success;
            canStopListening = success;
        }

        private async Task StopBotListening()
        {
            canStopListening = false;

            await _bot.StopListening();
            isBotListening = false;

            canStartListening = true;
        }

    }
}
