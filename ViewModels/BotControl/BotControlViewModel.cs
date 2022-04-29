using System;
using TextGameRPG.Scripts.TelegramBot;
using ReactiveUI;
using TextGameRPG.Scripts.Utils;
using System.Reactive;

namespace TextGameRPG.ViewModels.BotControl
{
    internal class BotControlViewModel : ViewModelBase
    {
        private string _consoleOutput = string.Empty;
        private TelegramBot _bot;
        private bool _isBotListening;        

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

        public ReactiveCommand<Unit, Unit> startListening { get; }
        public ReactiveCommand<Unit, Unit> stopListening { get; }

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

        private void StartBotListening()
        {
            _bot.StartListeningAsync();
            isBotListening = true;
        }

        private void StopBotListening()
        {
            _bot.StopListening();
            isBotListening = false;
        }

    }
}
