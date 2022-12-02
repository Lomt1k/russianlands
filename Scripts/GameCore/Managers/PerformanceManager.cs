using System;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Managers
{
    public enum PerformanceState { Normal, Highload, Busy }

    public class PerformanceManager : GlobalManager
    {
        private TelegramBot _bot;
        
        // cpu settings
        public int cpuUsageLimit { get; }
        public int cpuUsageHighload { get; }
        public int responseDelayWhenCpuHighload { get; }

        // memory settings 
        public int memoryUsageLimit { get; }
        public int memoryUsageHighload { get; }
        public int sessionTimeoutDefault { get; }
        public int sessionTimeoutWhenMemoryHighload { get; }

        public PerformanceState currentCpuState { get; private set; }
        public PerformanceState currentMemoryState { get; private set; }
        public PerformanceState currentState
        {
            get
            {
                return currentCpuState == PerformanceState.Busy || currentMemoryState == PerformanceState.Busy ? PerformanceState.Busy
                    : currentCpuState == PerformanceState.Highload || currentMemoryState == PerformanceState.Highload ? PerformanceState.Highload
                    : PerformanceState.Normal;
            }
        }

        public Action<PerformanceManager>? onStateUpdate;


        public PerformanceManager()
        {
            _bot = TelegramBot.instance;

            var config = _bot.config;

            cpuUsageLimit = config.cpuUsageLimitInPercents;
            cpuUsageHighload = config.cpuUsageToHighloadState;
            responseDelayWhenCpuHighload = config.responceMsDelayWhenCpuHighload;

            memoryUsageLimit = config.memoryUsageLimitInMegabytes;
            memoryUsageHighload = config.memoryUsageToHighloadState;
            sessionTimeoutDefault = config.sessionTimeoutInMinutes;
            sessionTimeoutWhenMemoryHighload = config.sessionTimeoutInMinutesWhenMemoryHighoad;

            UpdateCurrentState(PerformanceMonitor.cpuUsage, PerformanceMonitor.memoryUsage);
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            PerformanceMonitor.onUpdate += OnPerformanceUpdate;
        }

        private void UnsubscribeEvents()
        {
            PerformanceMonitor.onUpdate -= OnPerformanceUpdate;
        }

        private void OnPerformanceUpdate(double cpuUsage, double memoryUsage)
        {
            UpdateCurrentState(cpuUsage, memoryUsage);
        }

        private void UpdateCurrentState(double cpuUsage, double memoryUsage)
        {
            UpdateCpuState(cpuUsage);
            UpdateMemoryState(memoryUsage);
            onStateUpdate?.Invoke(this);
        }

        private void UpdateCpuState(double cpuUsage)
        {
            currentCpuState = cpuUsage > cpuUsageLimit ? PerformanceState.Busy
                : cpuUsage > cpuUsageHighload ? PerformanceState.Highload
                : PerformanceState.Normal;
        }

        private void UpdateMemoryState(double memoryUsage)
        {
            currentMemoryState = memoryUsage > memoryUsageLimit ? PerformanceState.Busy
                : memoryUsage > memoryUsageHighload ? PerformanceState.Highload
                : PerformanceState.Normal;
        }

        public int GetCurrentResponseDelay()
        {
            return currentCpuState == PerformanceState.Normal ? 0 : responseDelayWhenCpuHighload;
        }

        public int GetCurrentSessionTimeout()
        {
            return currentMemoryState == PerformanceState.Normal ? sessionTimeoutDefault : sessionTimeoutWhenMemoryHighload;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnsubscribeEvents();
            onStateUpdate = null;
        }

    }
}
