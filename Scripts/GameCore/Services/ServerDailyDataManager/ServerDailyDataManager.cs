using SQLite;
using System;
using System.Threading;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Services
{
    public class ServerDailyDataManager : Service
    {
        private static SQLiteAsyncConnection db => BotController.dataBase.db;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public DateTime lastDate { get; private set; }
        public event Action? onStartNewDay;

        public override async Task OnBotStarted()
        {
            lastDate = await GetDateValue("lastDate", DateTime.MinValue).FastAwait();
            _cts = new CancellationTokenSource();
            WaitNextDay(_cts.Token);
        }

        public override Task OnBotStopped()
        {
            _cts.Cancel();
            return Task.CompletedTask;
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
                    lastDate = lastDate.AddDays(1);
                    await StartNewDay().FastAwait();
                }
                var nextDate = lastDate.AddDays(1);
                var secondsToWait = (nextDate - now).TotalSeconds;
                var delay = (int)(secondsToWait * 1000) + 100;
                await Task.Delay(delay).FastAwait();
            }
        }

        private async Task StartNewDay()
        {
            await db.DeleteAllAsync<ServerDailyData>().FastAwait();
            await SetDateValue("lastDate", lastDate).FastAwait();
            onStartNewDay?.Invoke();
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
