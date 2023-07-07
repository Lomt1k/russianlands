using DynamicData;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using SQLite;
using StatViewer.Scripts.Metrics;
using StatViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StatViewer.Scripts;
internal static class StatDataBase
{
    public const string statisticsFolder = "statistics";
    public static readonly string cacheFolder = Path.Combine(statisticsFolder, "cache");
    public static readonly string dataBasePath = Path.Combine(cacheFolder, "cache.sqlite");

    public static SQLiteAsyncConnection db { get; private set; }

    public static async Task ConnectAsync()
    {
        if (!Directory.Exists(cacheFolder))
        {
            Directory.CreateDirectory(cacheFolder);
        }
        db = new SQLiteAsyncConnection(dataBasePath);
        await db.CreateTableAsync<ManifestData>();
        await db.CreateTableAsync<ProfileDailyStatData>();
    }

    public static async Task RefreshCache()
    {
        var allFiles = Directory.EnumerateFiles(statisticsFolder, "*.sqlite");
        foreach (var file in allFiles)
        {
            var fileInfo = new FileInfo(file);
            await RefreshFileIfNeed(fileInfo);
        }
    }

    private static async Task RefreshFileIfNeed(FileInfo fileInfo)
    {
        var manifestData = await db.Table<ManifestData>().Where(x => x.fileName == fileInfo.Name).FirstOrDefaultAsync();
        if (manifestData is not null)
        {
            if (manifestData.fileSize == fileInfo.Length)
            {
                return;
            }
        }

        manifestData = new ManifestData
        {
            fileName = fileInfo.Name,
            fileSize = fileInfo.Length,
        };

        var loadDb = new SQLiteAsyncConnection(fileInfo.FullName);
        var allData = await loadDb.Table<ProfileDailyStatData>().ToArrayAsync();
        foreach (var data in allData)
        {
            await db.InsertOrReplaceAsync(data);
        }
        await db.InsertOrReplaceAsync(manifestData);
    }

    public static async Task ShowStats(MetricType metricType, FilterModel[] filters)
    {
        filters = filters.Where(x => !x.isEmpty).ToArray();
        var headers = new List<string>();
        var table = new List<List<string>>();

        var task = metricType switch
        {
            MetricType.DailyActiveUsers => PrepareDAU(headers, table),
            MetricType.MonthActiveUsers => PrepareMAU(headers, table),
            MetricType.Revenue => PrepareRevenue(headers, table),
            MetricType.AverageRevenuePerUser => PrepareARPU(headers, table),
            MetricType.AverageRevenuePerPayingUser => PrepareARPPU(headers, table),

            // with filters
            MetricType.Retention => PrepareRetention(headers, table, filters),

            _ => Task.Delay(100)
        };
        await task;

        ResultsViewModel.instance.SetData(metricType.ToString(), headers, table);
    }

    private static async Task PrepareDAU(List<string> headers, List<List<string>> table)
    {
        headers.AddRange(new[] { "Date", "DAU" });
        var allData = await db.Table<ProfileDailyStatData>().ToArrayAsync();
        var minDate = allData.Min(x => x.date);
        var maxDate = allData.Max(x => x.date);
        var daysCount = (maxDate - minDate).Days + 1;

        var date = maxDate;
        for (var i = 0; i < daysCount; i++)
        {
            var dau = allData.Count(x => x.date == date);
            table.Add(new List<string>() { date.ToLongDateString(), dau.ToString() });
            date = date.AddDays(-1);
        }
    }

    private static async Task PrepareMAU(List<string> headers, List<List<string>> table)
    {
        headers.AddRange(new[] { "Month", "MAU" });
        var allData = await db.Table<ProfileDailyStatData>().ToArrayAsync();
        var minDate = allData.Min(x => x.date);
        var maxDate = allData.Max(x => x.date);
        var daysCount = (maxDate - minDate).Days + 1;

        var date = maxDate;
        var lastMonth = date.Month + 1;
        for (var i = 0; i < daysCount; i++)
        {
            if (date.Month == lastMonth)
            {
                continue;
            }
            lastMonth = date.Month;

            var selectedData = allData.Where(x => x.date.Year == date.Year && x.date.Month == date.Month).ToArray();
            var hashset = new HashSet<long>();
            foreach (var data in selectedData)
            {
                hashset.Add(data.dbid);
            }
            string monthName = date.ToString("MMMM", CultureInfo.InvariantCulture);
            table.Add(new List<string>() { $"{date.Year}-{monthName}", hashset.Count.ToString() });
            date = date.AddDays(-1);
        }
    }

    private static async Task PrepareRevenue(List<string> headers, List<List<string>> table)
    {
        headers.AddRange(new[] { "Date", "Revenue" });
        var allData = await db.Table<ProfileDailyStatData>().ToArrayAsync();
        var minDate = allData.Min(x => x.date);
        var maxDate = allData.Max(x => x.date);
        var daysCount = (maxDate - minDate).Days + 1;

        var date = maxDate;
        for (var i = 0; i < daysCount; i++)
        {
            var revenue = allData.Sum(x => x.revenueRUB);
            table.Add(new List<string>() { date.ToLongDateString(), $"RUB {revenue}" });
            date = date.AddDays(-1);
        }
    }

    private static async Task PrepareARPU(List<string> headers, List<List<string>> table)
    {
        headers.AddRange(new[] { "Date", "ARPU" });
        var allData = await db.Table<ProfileDailyStatData>().ToArrayAsync();
        var minDate = allData.Min(x => x.date);
        var maxDate = allData.Max(x => x.date);
        var daysCount = (maxDate - minDate).Days + 1;

        var date = maxDate;
        for (var i = 0; i < daysCount; i++)
        {
            var revenue = allData.Sum(x => x.revenueRUB);
            var dau = allData.Count(x => x.date == date);
            var arpu = dau > 0 ? (double)revenue / dau : 0;
            table.Add(new List<string>() { date.ToLongDateString(), $"RUB {arpu:F2}" });
            date = date.AddDays(-1);
        }
    }

    private static async Task PrepareARPPU(List<string> headers, List<List<string>> table)
    {
        headers.AddRange(new[] { "Date", "ARPPU" });
        var allData = await db.Table<ProfileDailyStatData>().ToArrayAsync();
        var minDate = allData.Min(x => x.date);
        var maxDate = allData.Max(x => x.date);
        var daysCount = (maxDate - minDate).Days + 1;

        var date = maxDate;
        for (var i = 0; i < daysCount; i++)
        {
            var revenue = allData.Sum(x => x.revenueRUB);
            var payersCount = allData.Count(x => x.date == date && x.revenueRUB > 0);
            var arppu = payersCount > 0 ? (double)revenue / payersCount : 0;
            table.Add(new List<string>() { date.ToLongDateString(), $"RUB {arppu:F2}" });
            date = date.AddDays(-1);
        }
    }

    private static async Task PrepareRetention(List<string> headers, List<List<string>> table, FilterModel[] filters)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var zeroDayDAUs = new List<int>();
        var maxDay = 0;
        foreach (var data in filteredData.Values)
        {
            var dau = data.Count(x => x.daysAfterRegistration == 0);
            zeroDayDAUs.Add(dau);

            var filterMaxDay = 0;
            try
            {
                data.Max(x => x.daysAfterRegistration);
            }
            catch (Exception)
            {
                // ignored
            }
            maxDay = Math.Max(maxDay, filterMaxDay);
        }

        for (var day = 0; day <= maxDay; day++)
        {
            var rowData = GetRowData(filteredData.Values, day, zeroDayDAUs);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int dayAfterRegistration, List<int> zeroDayDAUs)
        {
            var rowData = new List<string> { $"Day {dayAfterRegistration}" };
            int i = 0;
            foreach (var data in filteredDatas)
            {
                var dau = data.Count(x => x.daysAfterRegistration == dayAfterRegistration);
                var zeroDayDAU = zeroDayDAUs[i];
                var retention = zeroDayDAU > 0 ? (double)dau / zeroDayDAU * 100 : 0;
                rowData.Add($"{retention:F1}%");
                i++;
            }
            return rowData;
        }
    }

    private static async Task GetFilteredData(Dictionary<string, ProfileDailyStatData[]> destination, FilterModel[] filters)
    {
        foreach (var filter in filters)
        {
            var regInfo = filter.regInfo.Trim();
            var regVersion = filter.regVersion.Trim();

            var name = filter.ToString();
            var data = await db.Table<ProfileDailyStatData>()
                .Where(x => x.regInfo.Contains(regInfo) && x.regVersion.StartsWith(regVersion))
                .ToArrayAsync();

            destination.TryAdd(name, data);
        }
    }

}
