namespace StatViewer.Scripts.Metrics;

public enum MetricType
{
    DailyActiveUsers,
    MonthActiveUsers,
    Revenue,
    AverageRevenuePerUser,
    AverageRevenuePerPayingUser,

    // with filters
    Retention,
}

public static class MetricTypeExtensions
{
    public static bool IsSupportFilters(this MetricType metricType)
    {
        return metricType switch
        {
            MetricType.DailyActiveUsers => false,
            MetricType.MonthActiveUsers => false,
            MetricType.Revenue => false,
            MetricType.AverageRevenuePerUser => false,
            MetricType.AverageRevenuePerPayingUser => false,
            _ => true
        };
    }
}