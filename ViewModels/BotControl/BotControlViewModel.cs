using System;
using TextGameRPG.Scripts.Bot;
using ReactiveUI;
using TextGameRPG.Scripts.Utils;
using System.Reactive;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.ViewModels.BotControl
{
    public class BotControlViewModel : ViewModelBase
    {
        private static readonly PerformanceManager performanceManager = Services.Get<PerformanceManager>();

        private string _consoleOutput = string.Empty;
        private bool _isBotListening;
        private bool _canStartListening = true;
        private bool _canStopListening;

        private string _cpuUsageStat;
        private string _memoryUsageStat;
        private string _performanceStatus = "Status: -";

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

        public BotControlViewModel()
        {
            RedirectConsoleOutput();
            Program.logger.Info($"Selected bot data: {BotController.dataPath}");

            startListening = ReactiveCommand.Create(StartBotListening);
            stopListening = ReactiveCommand.Create(StopBotListening);

            UpdatePerformanceStats(new PerformanceInfo());
            PerformanceMonitor.onUpdate += UpdatePerformanceStats;
            performanceManager.onStateUpdate += UpdatePerformanceStatus;
        }

        private void RedirectConsoleOutput()
        {
            var textWriter = new TextWriterToString((output) => consoleOutput = output);
            Console.SetOut(textWriter);
        }

        private async Task StartBotListening()
        {
            canStartListening = false;

            bool success = await BotController.StartListening().FastAwait();
            isBotListening = success;

            canStartListening = !success;
            canStopListening = success;
        }

        private async Task StopBotListening()
        {
            canStopListening = false;

            await BotController.StopListening().FastAwait();
            isBotListening = false;
            canStartListening = true;
            performanceStatus = "Current status: -";
        }

        private void UpdatePerformanceStats(PerformanceInfo info)
        {
            cpuUsageStat = $"CPU: {info.applicationCpuUsage:F1}%";
            memoryUsageStat = $"RAM: {info.applicationRamUsage:F0} MB";
        }

        private void UpdatePerformanceStatus(PerformanceState state)
        {
            performanceStatus = $"Status: {state}";
        }

    }
}
