using HellBrick.Collections;
using log4net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.Utils.AsyncLogger;

public enum LogLevel : byte
{
    Info = 0,
    Debug = 1,
    Warn = 2,
    Error = 3,
    Fatal = 4,
}

public record struct LogRecord(LogLevel level, string message);

public class AsyncLogger
{
    private const int delayBeforeNextWrite = 15;

    private readonly AsyncQueue<LogRecord> _queue = new();
    private readonly static ILog _logger = LogManager.GetLogger(typeof(Program));

    public AsyncLogger()
    {
        Task.Run(WriteLogsLoop);
    }

    public void Info(object message)
    {
        var record = new LogRecord(LogLevel.Info, message.ToString() ?? "NULL");
        _queue.Add(record);
    }

    public void Debug(object message)
    {
        var record = new LogRecord(LogLevel.Debug, message.ToString() ?? "NULL");
        _queue.Add(record);
    }

    public void Warn(object message)
    {
        var record = new LogRecord(LogLevel.Warn, message.ToString() ?? "NULL");
        _queue.Add(record);
    }

    public void Error(object message)
    {
        var record = new LogRecord(LogLevel.Error, message.ToString() ?? "NULL");
        _queue.Add(record);
    }

    public void Fatal(object message)
    {
        var record = new LogRecord(LogLevel.Fatal, message.ToString() ?? "NULL");
        _queue.Add(record);
    }

    private async Task WriteLogsLoop()
    {
        while (true)
        {
            await WriteLogsFromQueue().FastAwait();
            await Task.Delay(delayBeforeNextWrite).FastAwait();
        }
    }

    private async Task WriteLogsFromQueue()
    {
        try
        {
            while (_queue.Count > 0)
            {
                var log = await _queue.TakeAsync().FastAwait();
                switch (log.level)
                {
                    case LogLevel.Info:
                        _logger.Info(log.message);
                        break;
                    case LogLevel.Debug:
                        _logger.Debug(log.message);
                        break;
                    case LogLevel.Warn:
                        _logger.Warn(log.message);
                        break;
                    case LogLevel.Error:
                        _logger.Error(log.message);
                        break;
                    case LogLevel.Fatal:
                        _logger.Fatal(log.message);
                        break;
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Catched exception in AsynLogger:\n" + ex);
        }
    }

}
