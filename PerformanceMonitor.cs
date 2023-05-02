using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TextGameRPG;

public struct PerformanceInfo
{
    public double applicationCpuUsage;
    public double applicationRamUsage;
    public double totalRamUsage;
    public double totalRamSize;

    public double totalRamUsagePercents => totalRamSize > 0 ? totalRamUsage / totalRamSize * 100 : 0;

    public PerformanceInfo(double _applicationCpuUsage, double _applicationRamUsage, double _totalRamUsage, double _totalRamSize)
    {
        applicationCpuUsage = _applicationCpuUsage;
        applicationRamUsage = _applicationRamUsage;
        totalRamUsage = _totalRamUsage;
        totalRamSize = _totalRamSize;
    }
}

public static class PerformanceMonitor
{
    private const int BYTES_IN_MEGABYTE = 1024 * 1024;
    private const int UPDATE_DELAY_MS = 5_000;

    private static Process _process;
    private static DateTime _lastUpdateTime;
    private static double _lastProcessorTime;
    private static int _nextMsDelay = UPDATE_DELAY_MS;

    private static PerformanceInfo _info;


    /// <summary>
    /// Сколько ресурсов процессора занято приложением (%)
    /// </summary>
    public static double applicationCpuUsage => _info.applicationCpuUsage;
    /// <summary>
    /// Объём оперативной памяти, занятой приложением (MB)
    /// </summary>
    public static double applicationRamUsage => _info.applicationRamUsage;
    /// <summary>
    /// Общий объем использумой компьютером оперативной памяти (MB)
    /// </summary>
    public static double totalRamUsage => _info.totalRamUsage;
    /// <summary>
    /// Общий объем оперативной памяти компьютера (MB)
    /// </summary>
    public static double totalRamSize => _info.totalRamSize;
    /// <summary>
    /// Общий объем использумой компьютером оперативной памяти (%)
    /// </summary>
    public static double totalRamUsagePercents => _info.totalRamUsagePercents;


    /// <summary>
    /// On update: cpu usage (%), memory usage (Mb)
    /// </summary>
    public static Action<PerformanceInfo>? onUpdate;

    public static void Start()
    {
        Monitoring();
    }

    private static async void Monitoring()
    {
        _process = Process.GetCurrentProcess();
        _process.Refresh();
        _lastUpdateTime = DateTime.UtcNow;

        _lastProcessorTime = _process.TotalProcessorTime.TotalMilliseconds;
        while (true)
        {
            await Task.Delay(_nextMsDelay).FastAwait();
            _process.Refresh();

            // CPU
            var processorTime = _process.TotalProcessorTime.TotalMilliseconds;
            var totalMsUsed = processorTime - _lastProcessorTime;
            var cpuUpdateTime = DateTime.UtcNow;
            var totalMsPassed = (cpuUpdateTime - _lastUpdateTime).TotalMilliseconds;
            var appCpuUsage = totalMsUsed / (Environment.ProcessorCount * totalMsPassed) * 100;

            _lastUpdateTime = cpuUpdateTime;
            _lastProcessorTime = processorTime;

            // Memory
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            var appRamUsage = (double)_process.WorkingSet64 / BYTES_IN_MEGABYTE;
            var totalRamUsage = (double)gcMemoryInfo.MemoryLoadBytes / BYTES_IN_MEGABYTE;
            var totalRamSize = (double)gcMemoryInfo.TotalAvailableMemoryBytes / BYTES_IN_MEGABYTE;

            // Update current info
            _info = new PerformanceInfo(appCpuUsage, appRamUsage, totalRamUsage, totalRamSize);
            NotifyListeners(_info);

            //calculate next delay
            var nextUpdateTime = cpuUpdateTime.AddMilliseconds(UPDATE_DELAY_MS);
            var targetDelay = (nextUpdateTime - DateTime.UtcNow).TotalMilliseconds;
            targetDelay = Math.Max(targetDelay, 0);
            _nextMsDelay = (int)Math.Round(targetDelay);
        }
    }

    private static void NotifyListeners(PerformanceInfo performanceInfo)
    {
        try
        {
            onUpdate?.Invoke(performanceInfo);
        }
        catch (Exception ex)
        {
            Program.logger.Error(ex);
        }
    }

}
