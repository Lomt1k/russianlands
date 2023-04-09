using System;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Managers
{
    public enum PerformanceState { Normal, Highload, Busy }

    public class PerformanceManager : GlobalManager
    {        
        // cpu settings
        public int cpuUsageLimit { get; private set; }
        public int cpuUsageHighload { get; private set; }
        public int responseDelayWhenCpuHighload { get; private set; }

        // memory settings 
        public int memoryUsageLimit { get; private set; }
        public int memoryUsageHighload { get; private set; }
        public int sessionTimeoutDefault { get; private set; }
        public int sessionTimeoutWhenMemoryHighload { get; private set; }

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
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            PerformanceMonitor.onUpdate += OnPerformanceUpdate;
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

        public override void OnBotStarted()
        {
            var config = TelegramBot.instance.config;
            cpuUsageLimit = config.cpuUsageLimitInPercents;
            cpuUsageHighload = config.cpuUsageToHighloadState;
            responseDelayWhenCpuHighload = config.responceMsDelayWhenCpuHighload;

            memoryUsageLimit = config.memoryUsageLimitInMegabytes;
            memoryUsageHighload = config.memoryUsageToHighloadState;
            sessionTimeoutDefault = config.sessionTimeoutInMinutes;
            sessionTimeoutWhenMemoryHighload = config.sessionTimeoutInMinutesWhenMemoryHighoad;

            UpdateCurrentState(PerformanceMonitor.cpuUsage, PerformanceMonitor.memoryUsage);
        }

    }
}
