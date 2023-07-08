using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.GameData;
using Microsoft.CodeAnalysis;
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

    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    public static SQLiteAsyncConnection db { get; private set; }

    static StatDataBase()
    {
        gameDataHolder.LoadAllData();
    }

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
            MetricType.Daily_Revenue => PrepareDailyRevenue(headers, table),
            MetricType.Daily_AverageRevenuePerUser => PrepareDailyARPU(headers, table),
            MetricType.Daily_AverageRevenuePerPayingUser => PrepareDailyARPPU(headers, table),

            // with filters
            MetricType.Filters_DailyActiveUsers => PrepareFilteredDAU(headers, table, filters),
            MetricType.Filters_Retention => PrepareRetention(headers, table, filters, isDeep: false),
            MetricType.Filters_DeepRetention => PrepareRetention(headers, table, filters, isDeep: true),
            MetricType.Filters_AverageTime => PrepareAverageTime(headers, table, filters, isDeep: false),
            MetricType.Filters_DeepAverageTime => PrepareAverageTime(headers, table, filters, isDeep: true),
            MetricType.Filters_Revenue => PrepareFilteredRevenue(headers, table, filters),
            MetricType.Filters_AverageRevenuePerUser => PrepareFilteredARPU(headers, table, filters),
            MetricType.Filters_AverageRevenuePerPayingUser => PrepareFilteredARPPU(headers, table, filters),
            MetricType.Filters_PayingConversion => PreparePayingConversion(headers, table, filters),
            MetricType.Filters_QuestProgress => PrepareQuestProgress(headers, table, filters),
            MetricType.Filters_PlayerLevelConversion => PreparePlayerLevelConversion(headers, table, filters),
            MetricType.Filters_TownhallConversion => PrepareTownhallConversion(headers, table, filters),

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

    private static async Task PrepareDailyRevenue(List<string> headers, List<List<string>> table)
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

    private static async Task PrepareDailyARPU(List<string> headers, List<List<string>> table)
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

    private static async Task PrepareDailyARPPU(List<string> headers, List<List<string>> table)
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

    #region WITH FILTERS

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

    private static async Task PrepareFilteredDAU(List<string> headers, List<List<string>> table, FilterModel[] filters)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var maxDay = 0;
        foreach (var data in filteredData.Values)
        {
            var filterMaxDay = data.Length > 0 ? data.Max(x => x.daysAfterRegistration) : 0;
            maxDay = Math.Max(maxDay, filterMaxDay);
        }

        for (var day = 0; day <= maxDay; day++)
        {
            var rowData = GetRowData(filteredData.Values, day);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int dayAfterRegistration)
        {
            var rowData = new List<string> { $"Day {dayAfterRegistration}" };
            foreach (var data in filteredDatas)
            {
                var dau = data.Count(x => x.daysAfterRegistration == dayAfterRegistration);
                rowData.Add(dau.ToString());
            }
            return rowData;
        }
    }

    private static async Task PrepareRetention(List<string> headers, List<List<string>> table, FilterModel[] filters, bool isDeep)
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

            var filterMaxDay = data.Length > 0 ? data.Max(x => x.daysAfterRegistration) : 0;
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
                var dau = isDeep
                    ? data.Count(x => x.daysAfterRegistration == dayAfterRegistration && x.battlesCount >= 3) 
                    : data.Count(x => x.daysAfterRegistration == dayAfterRegistration);
                var zeroDayDAU = zeroDayDAUs[i];
                var retention = zeroDayDAU > 0 ? (double)dau / zeroDayDAU * 100 : 0;
                rowData.Add($"{retention:F1}%");
                i++;
            }
            return rowData;
        }
    }

    private static async Task PrepareAverageTime(List<string> headers, List<List<string>> table, FilterModel[] filters, bool isDeep)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var maxDay = 0;
        foreach (var data in filteredData.Values)
        {
            var filterMaxDay = data.Length > 0 ? data.Max(x => x.daysAfterRegistration) : 0;
            maxDay = Math.Max(maxDay, filterMaxDay);
        }

        for (var day = 0; day <= maxDay; day++)
        {
            var rowData = GetRowData(filteredData.Values, day);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int dayAfterRegistration)
        {
            var rowData = new List<string> { $"Day {dayAfterRegistration}" };
            foreach (var data in filteredDatas)
            {
                var sortedData = isDeep
                    ? data.Where(x => x.daysAfterRegistration == dayAfterRegistration && x.battlesCount >= 3).ToArray()
                    : data.Where(x => x.daysAfterRegistration == dayAfterRegistration).ToArray();

                var dau = sortedData.Length;
                var fulltime = dau > 0 ? sortedData.Sum(x => x.activityInSeconds) : 0;
                var averageTime = dau > 0 ? fulltime / dau : 0;
                var timeSpan = TimeSpan.FromSeconds(averageTime);
                rowData.Add(timeSpan.ToString());
            }
            return rowData;
        }
    }

    private static async Task PrepareFilteredRevenue(List<string> headers, List<List<string>> table, FilterModel[] filters)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var maxDay = 0;
        foreach (var data in filteredData.Values)
        {
            var filterMaxDay = data.Length > 0 ? data.Max(x => x.daysAfterRegistration) : 0;
            maxDay = Math.Max(maxDay, filterMaxDay);
        }

        for (var day = 0; day <= maxDay; day++)
        {
            var rowData = GetRowData(filteredData.Values, day);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int dayAfterRegistration)
        {
            var rowData = new List<string> { $"Day {dayAfterRegistration}" };
            foreach (var data in filteredDatas)
            {
                var sortedData = data.Where(x => x.daysAfterRegistration == dayAfterRegistration).ToArray();
                var revenue = sortedData.Sum(x => x.revenueRUB);
                rowData.Add($"RUB {revenue}");
            }
            return rowData;
        }
    }

    private static async Task PrepareFilteredARPU(List<string> headers, List<List<string>> table, FilterModel[] filters)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var maxDay = 0;
        foreach (var data in filteredData.Values)
        {
            var filterMaxDay = data.Length > 0 ? data.Max(x => x.daysAfterRegistration) : 0;
            maxDay = Math.Max(maxDay, filterMaxDay);
        }

        for (var day = 0; day <= maxDay; day++)
        {
            var rowData = GetRowData(filteredData.Values, day);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int dayAfterRegistration)
        {
            var rowData = new List<string> { $"Day {dayAfterRegistration}" };
            foreach (var data in filteredDatas)
            {
                var sortedData = data.Where(x => x.daysAfterRegistration == dayAfterRegistration).ToArray();
                var dau = sortedData.Length;
                var revenue = sortedData.Sum(x => x.revenueRUB);
                var arpu = dau > 0 ? (double)revenue / dau : 0;
                rowData.Add($"RUB {revenue:F2}");
            }
            return rowData;
        }
    }

    private static async Task PrepareFilteredARPPU(List<string> headers, List<List<string>> table, FilterModel[] filters)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var maxDay = 0;
        foreach (var data in filteredData.Values)
        {
            var filterMaxDay = data.Length > 0 ? data.Max(x => x.daysAfterRegistration) : 0;
            maxDay = Math.Max(maxDay, filterMaxDay);
        }

        for (var day = 0; day <= maxDay; day++)
        {
            var rowData = GetRowData(filteredData.Values, day);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int dayAfterRegistration)
        {
            var rowData = new List<string> { $"Day {dayAfterRegistration}" };
            foreach (var data in filteredDatas)
            {
                var sortedData = data.Where(x => x.daysAfterRegistration == dayAfterRegistration).ToArray();
                var donatersCount = sortedData.Count(x => x.revenueRUB > 0);
                var revenue = sortedData.Sum(x => x.revenueRUB);
                var arpu = donatersCount > 0 ? (double)revenue / donatersCount : 0;
                rowData.Add($"RUB {revenue:F2}");
            }
            return rowData;
        }
    }

    private static async Task PreparePayingConversion(List<string> headers, List<List<string>> table, FilterModel[] filters)
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

            var filterMaxDay = data.Length > 0 ? data.Max(x => x.daysAfterRegistration) : 0;
            maxDay = Math.Max(maxDay, filterMaxDay);
        }

        var donatersCache = new List<HashSet<long>>();
        for (int i = 0; i < zeroDayDAUs.Count; i++)
        {
            donatersCache.Add(new HashSet<long>());
        }
        
        for (var day = 0; day <= maxDay; day++)
        {
            var rowData = GetRowData(filteredData.Values, day, zeroDayDAUs, donatersCache);
            table.Add(rowData);
        }
        
        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int dayAfterRegistration, List<int> zeroDayDAUs, List<HashSet<long>> donatersCache)
        {
            var rowData = new List<string> { $"Day {dayAfterRegistration}" };
            int i = 0;
            foreach (var data in filteredDatas)
            {
                var zeroDayDAU = zeroDayDAUs[i];
                var donaters = data.Where(x => x.daysAfterRegistration == dayAfterRegistration && x.revenueRUB > 0).Select(x => x.dbid);
                foreach (var donater in donaters)
                {
                    donatersCache[i].Add(donater);
                }
                
                var conversion = zeroDayDAU > 0 ? (double)donatersCache[i].Count / zeroDayDAU * 100 : 0;
                rowData.Add($"{conversion:F2}%");
                i++;
            }
            return rowData;
        }
    }

    private static async Task PrepareQuestProgress(List<string> headers, List<List<string>> table, FilterModel[] filters)
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
        }

        var questId = QuestId.MainQuest;
        var stage = gameDataHolder.quests[questId].stages.First();
        while (true)
        {
            var nextStageInfo = TryGetNextStage(questId, stage);
            if (nextStageInfo is null)
            {
                return;
            }
            questId = nextStageInfo.Value.questId;
            stage = nextStageInfo.Value.stage;

            var rowData = GetRowData(filteredData.Values, questId, stage, zeroDayDAUs);
            table.Add(rowData);
        }


        (QuestId questId, QuestStage stage)? TryGetNextStage(QuestId lastQuestId, QuestStage lastStage)
        {
            var quest = gameDataHolder.quests[lastQuestId];
            if (quest.stages.Last() == lastStage)
            {
                if (lastQuestId == QuestId.Loc_07)
                {
                    return null;
                }
                var nextQuestId = lastQuestId + 1;
                var nextStage = gameDataHolder.quests[nextQuestId].stages.First();
                return (nextQuestId, nextStage);
            }
            bool lastStageFound = false;
            foreach (var stage in quest.stages)
            {
                if (lastStageFound)
                {
                    return (lastQuestId, stage);
                }
                lastStageFound = stage == lastStage;
            }
            return null;
        }

        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, QuestId questId, QuestStage stage, List<int> zeroDayDAUs)
        {
            var comment = stage.comment.Length > 24 ? stage.comment.Substring(0, 24) : stage.comment;
            comment = comment.Replace(Environment.NewLine, " ");
            var rowData = new List<string> { $"{questId} stage {stage.id} | {comment}" };
            int i = 0;
            foreach (var data in filteredDatas)
            {
                var users = data.Where(x => x.currentQuest > questId || (x.currentQuest == questId && x.currentQuestStage >= stage.id) ).Select(x => x.dbid).ToHashSet();
                var zeroDayDAU = zeroDayDAUs[i];
                var conversion = zeroDayDAU > 0 ? (double)users.Count / zeroDayDAU * 100 : 0;
                rowData.Add($"{conversion:F1}%");
                i++;
            }
            return rowData;
        }
    }

    private static async Task PreparePlayerLevelConversion(List<string> headers, List<List<string>> table, FilterModel[] filters)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var zeroDayDAUs = new List<int>();
        foreach (var data in filteredData.Values)
        {
            var dau = data.Count(x => x.daysAfterRegistration == 0);
            zeroDayDAUs.Add(dau);
        }

        for (var level = 1; level <= 35; level++)
        {
            var rowData = GetRowData(filteredData.Values, level, zeroDayDAUs);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int level, List<int> zeroDayDAUs)
        {
            var rowData = new List<string> { $"Level {level}" };
            int i = 0;
            foreach (var data in filteredDatas)
            {
                var usersCount = data.Where(x => x.playerLevel >= level).Select(x => x.dbid).ToHashSet().Count;
                var zeroDayDAU = zeroDayDAUs[i];
                var retention = zeroDayDAU > 0 ? (double)usersCount / zeroDayDAU * 100 : 0;
                rowData.Add($"{retention:F1}%");
                i++;
            }
            return rowData;
        }
    }

    private static async Task PrepareTownhallConversion(List<string> headers, List<List<string>> table, FilterModel[] filters)
    {
        var filteredData = new Dictionary<string, ProfileDailyStatData[]>();
        await GetFilteredData(filteredData, filters);

        headers.Add("Day");
        headers.AddRange(filteredData.Keys);

        var zeroDayDAUs = new List<int>();
        foreach (var data in filteredData.Values)
        {
            var dau = data.Count(x => x.daysAfterRegistration == 0);
            zeroDayDAUs.Add(dau);
        }

        for (var townhall = 1; townhall <= 8; townhall++)
        {
            var rowData = GetRowData(filteredData.Values, townhall, zeroDayDAUs);
            table.Add(rowData);
        }


        List<string> GetRowData(IEnumerable<ProfileDailyStatData[]> filteredDatas, int townhall, List<int> zeroDayDAUs)
        {
            var rowData = new List<string> { $"Townhall {townhall}" };
            int i = 0;
            foreach (var data in filteredDatas)
            {
                var usersCount = data.Where(x => x.townhallLevel >= townhall).Select(x => x.dbid).ToHashSet().Count;
                var zeroDayDAU = zeroDayDAUs[i];
                var retention = zeroDayDAU > 0 ? (double)usersCount / zeroDayDAU * 100 : 0;
                rowData.Add($"{retention:F1}%");
                i++;
            }
            return rowData;
        }
    }



    #endregion



}
