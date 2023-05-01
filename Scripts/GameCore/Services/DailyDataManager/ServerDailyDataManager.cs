using SQLite;
using System;
using System.Threading;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Services.DailyDataManagers
{
    public class ServerDailyDataManager : Service
    {
        private static SQLiteAsyncConnection db => BotController.dataBase.db;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public DateTime lastDate { get; private set; }
        public event Action<DateTime,DateTime>? onStartNewDay;
        public event Action? onStartWithOldDay;

        public override async Task OnBotStarted()
        {
            lastDate = await GetDateValue("lastDate", DateTime.MinValue).FastAwait();
            _cts = new CancellationTokenSource();

            var now = DateTime.UtcNow;
            var daysPassed = (now - lastDate).Days;
            if (daysPassed > 0)
            {
                var newDate = lastDate.AddDays(daysPassed);
                await StartNewDay(lastDate, newDate).FastAwait();
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
                    await StartNewDay(lastDate, lastDate.AddDays(1)).FastAwait();
                }
                var nextDate = lastDate.AddDays(1);
                var secondsToWait = (nextDate - now).TotalSeconds;
                var delay = (int)(secondsToWait * 1000) + 100;
                await Task.Delay(delay).FastAwait();
            }
        }

        private async Task StartNewDay(DateTime oldDate, DateTime newDate)
        {
            lastDate = newDate;
            await db.DeleteAllAsync<ServerDailyData>().FastAwait();
            await SetDateValue("lastDate", lastDate).FastAwait();
            Program.logger.Info($"New day started! ({lastDate.AsDateString()})");
            onStartNewDay?.Invoke(oldDate, newDate);
        }

        #region setters

        public async Task SetStringValue(string key, string value)
        {
            var data = new ServerDailyData { key = key, value = value };
            await db.InsertOrReplaceAsync(data).FastAwait();
        }

        public async Task SetIntegerValue(string key, long value)
        {
            var data = new ServerDailyData { key = key, value = value.ToString() };
            await db.InsertOrReplaceAsync(data).FastAwait();
        }

        public async Task SetDoubleValue(string key, double value)
        {
            var data = new ServerDailyData { key = key, value = value.ToString() };
            await db.InsertOrReplaceAsync(data).FastAwait();
        }

        public async Task SetDateTimeValue(string key, DateTime value)
        {
            var data = new ServerDailyData { key = key, value = value.AsDateTimeString() };
            await db.InsertOrReplaceAsync(data).FastAwait();
        }

        public async Task SetDateValue(string key, DateTime value)
        {
            var data = new ServerDailyData { key = key, value = value.AsDateString() };
            await db.InsertOrReplaceAsync(data).FastAwait();
        }

        #endregion

        #region getters

        public async Task<string> GetStringValue(string key, string defaultValue)
        {
            var result = await db.GetOrNullAsync<ServerDailyData>(key);
            if (result == null)
            {
                await SetStringValue(key, defaultValue).FastAwait();
                return defaultValue;
            }
            return result.value;
        }

        public async Task<long> GetIntegerValue(string key, long defaultValue = 0)
        {
            var result = await db.GetOrNullAsync<ServerDailyData>(key);
            if (result == null)
            {
                await SetIntegerValue(key, defaultValue).FastAwait();
                return defaultValue;
            }
            return Convert.ToInt64(result.value);
        }

        public async Task<double> GetDoubleValue(string key, double defaultValue = 0)
        {
            var result = await db.GetOrNullAsync<ServerDailyData>(key);
            if (result == null)
            {
                await SetDoubleValue(key, defaultValue).FastAwait();
                return defaultValue;
            }
            return Convert.ToDouble(result.value);
        }

        public async Task<DateTime> GetDateTimeValue(string key, DateTime defaultValue)
        {
            var result = await db.GetOrNullAsync<ServerDailyData>(key);
            if (result == null)
            {
                await SetDateTimeValue(key, defaultValue).FastAwait();
                return defaultValue;
            }
            return result.value.AsDateTime();
        }

        public async Task<DateTime> GetDateValue(string key, DateTime defaultValue)
        {
            var result = await db.GetOrNullAsync<ServerDailyData>(key);
            if (result == null)
            {
                await SetDateValue(key, defaultValue).FastAwait();
                return defaultValue;
            }
            return result.value.AsDate();
        }

        #endregion


    }
}
