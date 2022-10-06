using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TextGameRPG
{
    public static class PerformanceMonitor
    {
        private const int BYTES_IN_MEGABYTE = 1024 * 1024;
        private const int UPDATE_DELAY_MS = 5_000;

        private static Process _process;
        private static DateTime _lastCpuUpdateTime;
        private static double _lastProcessorTime;
        private static int _nextMsDelay = UPDATE_DELAY_MS;

        public static bool isRunning { get; private set; }
        /// <summary>
        /// Сколько ресурсов процессора в % занято приложением
        /// </summary>
        public static double cpuUsage { get; private set; }
        /// <summary>
        /// Сколько оперативной памяти (в мегабайтах) занято приложением
        /// </summary>
        public static double memoryUsage { get; private set; }

        /// <summary>
        /// On update: cpu usage (%), memory usage (Mb)
        /// </summary>
        public static Action<double, double>? onUpdate;

        public static void Start()
        {
            if (isRunning)
                return;

            isRunning = true;
            Monitoring();
        }

        private static async void Monitoring()
        {
            _process = Process.GetCurrentProcess();
            _process.Refresh();
            _lastCpuUpdateTime = DateTime.UtcNow;
            _lastProcessorTime = _process.TotalProcessorTime.TotalMilliseconds;
            while (true)
            {
                await Task.Delay(_nextMsDelay);
                _process.Refresh();

                // CPU
                var processorTime = _process.TotalProcessorTime.TotalMilliseconds;
                var totalMsUsed = processorTime - _lastProcessorTime;
                var cpuUpdateTime = DateTime.UtcNow;
                var totalMsPassed = (cpuUpdateTime - _lastCpuUpdateTime).TotalMilliseconds;
                cpuUsage = totalMsUsed / (Environment.ProcessorCount * totalMsPassed) * 100;

                _lastCpuUpdateTime = cpuUpdateTime;
                _lastProcessorTime = processorTime;

                // Memory
                memoryUsage = (double)_process.WorkingSet64 / BYTES_IN_MEGABYTE;

                NotifyListeners();

                //calculate next delay
                var nextUpdateTime = cpuUpdateTime.AddMilliseconds(UPDATE_DELAY_MS);
                var targetDelay = (nextUpdateTime - DateTime.UtcNow).TotalMilliseconds;
                targetDelay = Math.Max(targetDelay, 0);
                _nextMsDelay = (int)Math.Round(targetDelay);
            }            
        }

        private static void NotifyListeners()
        {
            try
            {
                onUpdate?.Invoke(cpuUsage, memoryUsage);
            }
            catch (Exception ex)
            {
                Program.logger.Error(ex);
            }
        }

    }
}
