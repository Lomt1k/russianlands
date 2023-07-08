namespace StatViewer.Scripts.Metrics;

public enum MetricType
{
    DailyActiveUsers,
    MonthActiveUsers,
    Daily_Revenue,
    Daily_AverageRevenuePerUser,
    Daily_AverageRevenuePerPayingUser,

    // with filters
    Filters_DailyActiveUsers,
    Filters_Retention,
    Filters_DeepRetention,
    Filters_AverageTime,
    Filters_DeepAverageTime,    
    Filters_Revenue,
    Filters_AverageRevenuePerUser,
    Filters_AverageRevenuePerPayingUser,
    Filters_PayingConversion,
    Filters_QuestProgress,
    Filters_PlayerLevelConversion,
    Filters_TownhallConversion,
    Filters_AverageBattles,
    Filters_DeepAverageBattles,
    Filters_AverageArenaBattles,
    Filters_DeepAverageArenaBattles,
    Filters_AverageArenaResults,
    Filters_DeepAverageArenaResults,
    Filters_ArenaLeagueConversion,
}

public static class MetricTypeExtensions
{
    public static bool IsSupportFilters(this MetricType metricType)
    {
        return metricType switch
        {
            MetricType.DailyActiveUsers => false,
            MetricType.MonthActiveUsers => false,
            MetricType.Daily_Revenue => false,
            MetricType.Daily_AverageRevenuePerUser => false,
            MetricType.Daily_AverageRevenuePerPayingUser => false,
            _ => true
        };
    }
}