using System;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Managers
{
    public enum PerformanceState { Normal, Highload, Busy }

    public class PerformanceManager : Singletone
    {        
        // cpu settings
        public int cpuUsageHighload { get; private set; }
        public int responseDelayWhenCpuHighload { get; private set; }

        // memory settings 
        public int memoryUsageLimit { get; private set; }
        public int sessionTimeoutDefault { get; private set; }

        public PerformanceState currentState { get; private set; }

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
            currentState = memoryUsage >= memoryUsageLimit ? PerformanceState.Busy
                : cpuUsage >= cpuUsageHighload ? PerformanceState.Highload
                : PerformanceState.Normal;
            onStateUpdate?.Invoke(this);
        }

        public int GetCurrentResponseDelay()
        {
            return PerformanceMonitor.cpuUsage < cpuUsageHighload ? 0 : responseDelayWhenCpuHighload;
        }

        public override void OnBotStarted()
        {
            var config = BotConfig.instance;
            cpuUsageHighload = config.cpuUsageToHighloadState;
            responseDelayWhenCpuHighload = config.responceMsDelayWhenCpuHighload;
            memoryUsageLimit = config.memoryUsageLimitInMegabytes;
            sessionTimeoutDefault = config.sessionTimeoutInMinutes;

            UpdateCurrentState(PerformanceMonitor.cpuUsage, PerformanceMonitor.memoryUsage);
        }

    }
}
