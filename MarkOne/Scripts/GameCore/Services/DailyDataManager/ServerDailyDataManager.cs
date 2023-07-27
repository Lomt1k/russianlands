using SQLite;
using System;
using System.Threading;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Services.DailyDataManagers;

public class ServerDailyDataManager : Service
{
    private static SQLiteConnection db => BotController.dataBase.db;

    private CancellationTokenSource _cts = new CancellationTokenSource();

    public DateTime lastDate { get; private set; }
    public event Action<DateTime, DateTime>? onStartNewDay;
    public event Action? onStartWithOldDay;

    public override async Task OnBotStarted()
    {
        lastDate = GetDateValue("lastDate", DateTime.MinValue);
        _cts = new CancellationTokenSource();

        var now = DateTime.UtcNow;
        var daysPassed = (now - lastDate).Days;
        if (daysPassed > 0)
        {
            var newDate = lastDate.AddDays(daysPassed);
            StartNewDay(lastDate, newDate);
        }
        else
        {
            onStartWithOldDay?.Invoke();
        }

        WaitNextDay(_cts.Token);
    }

    public override Task OnBotStopped()
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }

    public TimeSpan GetTimeUntilNextDay()
    {
        var now = DateTime.UtcNow;
        var nextDay = lastDate.AddDays(1);
        return nextDay - now;
    }

    private async void WaitNextDay(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var now = DateTime.UtcNow;
            if ((now - lastDate).Days > 0)
            {
                StartNewDay(lastDate, lastDate.AddDays(1));
            }
            var nextDate = lastDate.AddDays(1);
            var secondsToWait = (nextDate - now).TotalSeconds;
            var delay = (int)(secondsToWait * 1000) + 100;
            await Task.Delay(delay).FastAwait();
        }
    }

    private void StartNewDay(DateTime oldDate, DateTime newDate)
    {
        lastDate = newDate;
        db.DeleteAll<ServerDailyData>();
        SetDateValue("lastDate", lastDate);
        Program.logger.Info($"New day started! ({lastDate.AsDateString()})");
        onStartNewDay?.Invoke(oldDate, newDate);
    }

    #region setters

    public void SetStringValue(string key, string value)
    {
        var data = new ServerDailyData { key = key, value = value };
        db.InsertOrReplace(data);
    }

    public void SetIntegerValue(string key, long value)
    {
        var data = new ServerDailyData { key = key, value = value.ToString() };
        db.InsertOrReplace(data);
    }

    public void SetDoubleValue(string key, double value)
    {
        var data = new ServerDailyData { key = key, value = value.ToString() };
        db.InsertOrReplace(data);
    }

    public void SetDateTimeValue(string key, DateTime value)
    {
        var data = new ServerDailyData { key = key, value = value.AsDateTimeString() };
        db.InsertOrReplace(data);
    }

    public void SetDateValue(string key, DateTime value)
    {
        var data = new ServerDailyData { key = key, value = value.AsDateString() };
        db.InsertOrReplace(data);
    }

    #endregion

    #region getters

    public string GetStringValue(string key, string defaultValue)
    {
        var result = db.GetOrNull<ServerDailyData>(key);
        if (result == null)
        {
            SetStringValue(key, defaultValue);
            return defaultValue;
        }
        return result.value;
    }

    public long GetIntegerValue(string key, long defaultValue = 0)
    {
        var result = db.GetOrNull<ServerDailyData>(key);
        if (result == null)
        {
            SetIntegerValue(key, defaultValue);
            return defaultValue;
        }
        return Convert.ToInt64(result.value);
    }

    public double GetDoubleValue(string key, double defaultValue = 0)
    {
        var result = db.GetOrNull<ServerDailyData>(key);
        if (result == null)
        {
            SetDoubleValue(key, defaultValue);
            return defaultValue;
        }
        return Convert.ToDouble(result.value);
    }

    public DateTime GetDateTimeValue(string key, DateTime defaultValue)
    {
        var result = db.GetOrNull<ServerDailyData>(key);
        if (result == null)
        {
            SetDateTimeValue(key, defaultValue);
            return defaultValue;
        }
        return result.value.AsDateTime();
    }

    public DateTime GetDateValue(string key, DateTime defaultValue)
    {
        var result = db.GetOrNull<ServerDailyData>(key);
        if (result == null)
        {
            SetDateValue(key, defaultValue);
            return defaultValue;
        }
        return result.value.AsDate();
    }

    #endregion


}
