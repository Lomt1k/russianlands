using System;
using TextGameRPG.Scripts.TelegramBot;
using ReactiveUI;
using TextGameRPG.Scripts.Utils;
using System.Reactive;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Managers;

namespace TextGameRPG.ViewModels.BotControl
{
    public class BotControlViewModel : ViewModelBase
    {
        private string _consoleOutput = string.Empty;
        private TelegramBot _bot;
        private bool _isBotListening;
        private bool _canStartListening = true;
        private bool _canStopListening;

        private string _cpuUsageStat;
        private string _memoryUsageStat;
        private string _performanceStatus = "Current status: -";

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

        public string cpuUsageStat
        {
            get => _cpuUsageStat;
            set => this.RaiseAndSetIfChanged(ref _cpuUsageStat, value);
        }
        public string memoryUsageStat
        {
            get => _memoryUsageStat;
            set => this.RaiseAndSetIfChanged(ref _memoryUsageStat, value);
        }
        public string performanceStatus
        {
            get => _performanceStatus;
            set => this.RaiseAndSetIfChanged(ref _performanceStatus, value);
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

            UpdatePerformanceStats(PerformanceMonitor.cpuUsage, PerformanceMonitor.memoryUsage);
            PerformanceMonitor.onUpdate += UpdatePerformanceStats;
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

            if (success)
            {
                var performanceManager = GlobalManagers.performanceManager;
                if (performanceManager != null)
                {
                    performanceManager.onStateUpdate += UpdatePerformanceStatus;
                }
            }
        }

        private async Task StopBotListening()
        {
            canStopListening = false;

            await _bot.StopListening();
            isBotListening = false;
            canStartListening = true;
            performanceStatus = "Current status: -";
        }

        private void UpdatePerformanceStats(double cpuUsage, double memoryUsage)
        {
            cpuUsageStat = $"CPU: {cpuUsage:F1}%";
            memoryUsageStat = $"RAM: {memoryUsage:F0} Mb";
        }

        private void UpdatePerformanceStatus(PerformanceManager performance)
        {
            var currentState = performance.currentState;
            var cpuState = performance.currentCpuState;
            var memoryState = performance.currentMemoryState;
            performanceStatus = $"Current status: {currentState}"
                + (currentState == PerformanceState.Normal ? string.Empty : $" (CPU: {cpuState}, RAM: {memoryState})");
        }

    }
}
