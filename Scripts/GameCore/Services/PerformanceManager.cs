using System;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Services
{
    public enum PerformanceState { Normal, Highload, ShutdownRequired }

    public struct PerformanceDebugInfo
    {
        public string cpuInfo;
        public string memoryInfo;
        public string totalMemoryInfo;

        public override string ToString()
        {
            return cpuInfo + '\n' + memoryInfo + '\n' + totalMemoryInfo;
        }
    }

    public class PerformanceManager : Service
    {        

        public PerformanceState currentState { get; private set; }
        public int currentResponceDelay { get; private set; }
        public PerformanceDebugInfo debugInfo { get; private set; }

        public Action<PerformanceState>? onStateUpdate;


        public PerformanceManager()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            PerformanceMonitor.onUpdate += OnPerformanceUpdate;
        }

        private void OnPerformanceUpdate(PerformanceInfo info)
        {
            currentState = GetActualState(info);
            debugInfo = GetDebugInfo(info);
            currentResponceDelay = currentState != PerformanceState.Normal ? BotConfig.instance.responceMsDelayWhenCpuHighload : 0;

            onStateUpdate?.Invoke(currentState);
        }

        private PerformanceState GetActualState(PerformanceInfo info)
        {
            var config = BotConfig.instance;
            if (config == null)
                return PerformanceState.Normal;

            var appRamLimit = config.appRamUsageLimitInMegabytes >  0 ? (int?)config.appRamUsageLimitInMegabytes : null;
            var totalRamLimitPercentage = config.totalRamUsageLimitInPercents > 0 ? (int?)config.totalRamUsageLimitInPercents : null;
            var cpuLimitPercentage = config.cpuUsageToHighloadStateInPercents > 0 ? (int?)config.cpuUsageToHighloadStateInPercents : null;

            if (appRamLimit.HasValue && info.applicationRamUsage >= appRamLimit)
            {
                return PerformanceState.ShutdownRequired;
            }
            if (totalRamLimitPercentage.HasValue && info.totalRamUsagePercents >= totalRamLimitPercentage)
            {
                return PerformanceState.ShutdownRequired;
            }
            if (cpuLimitPercentage.HasValue && info.applicationCpuUsage > cpuLimitPercentage)
            {
                return PerformanceState.Highload;
            }

            return PerformanceState.Normal;
        }

        private PerformanceDebugInfo GetDebugInfo(PerformanceInfo info)
        {
            var config = BotConfig.instance;
            if (config == null)
                return new PerformanceDebugInfo();

            var appRamLimit = config.appRamUsageLimitInMegabytes > 0 ? (int?)config.appRamUsageLimitInMegabytes : null;

            return new PerformanceDebugInfo
            {
                cpuInfo = $"CPU: {info.applicationCpuUsage:F1}%",
                memoryInfo = $"RAM: {info.applicationRamUsage:F0}" + (appRamLimit.HasValue ? $" / {appRamLimit:F0}" : string.Empty) + " MB",
                totalMemoryInfo = $"Total RAM: {info.totalRamUsage:F0} / {info.totalRamSize:F0} MB ({info.totalRamUsagePercents:F0}%)"
            };
        }

        public override void OnBotStarted(TelegramBot bot)
        {
            onStateUpdate?.Invoke(PerformanceState.Normal);
        }

    }
}
