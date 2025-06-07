using System.Linq.Expressions;
using WebApp.Enums;
using WebApp.Model;

namespace WebApp.Helpers;

public static class DataHelpers
{
    public static IQueryable<WeatherData> ApplyTimeRangeFilter(IQueryable<WeatherData> query, TimeRange timeRange)
    {
        DateTime cutoffTime = DateTime.UtcNow;

        switch (timeRange)
        {
            case TimeRange.Min5:
                cutoffTime = DateTime.UtcNow.AddMinutes(-5);
                break;
            case TimeRange.Min10:
                cutoffTime = DateTime.UtcNow.AddMinutes(-10);
                break;
            case TimeRange.Min30:
                cutoffTime = DateTime.UtcNow.AddMinutes(-30);
                break;
            case TimeRange.H1:
                cutoffTime = DateTime.UtcNow.AddHours(-1);
                break;
            case TimeRange.H24:
                cutoffTime = DateTime.UtcNow.AddHours(-24);
                break;
            case TimeRange.Mo1:
                cutoffTime = DateTime.UtcNow.AddMonths(-1);
                break;
            case TimeRange.Y1:
                cutoffTime = DateTime.UtcNow.AddYears(-1);
                break;
            case TimeRange.All:
                return query;
            default:
                throw new ArgumentException("Invalid time range specified. Valid options are: Min5, Min10, Min30, H1, H24, Mo1, Y1, All.");
        }

        return query.Where(wd => wd.Timestamp >= cutoffTime);
    }

    public static Expression<Func<WeatherData, object>> GetGroupBySelector(TimeRange timeRange)
    {
        switch (timeRange)
        {
            case TimeRange.Min5:
            case TimeRange.Min10:
                return wd => new
                {
                    wd.Timestamp.Value.Year,
                    wd.Timestamp.Value.Month,
                    wd.Timestamp.Value.Day,
                    wd.Timestamp.Value.Hour,
                    wd.Timestamp.Value.Minute,
                    wd.Timestamp.Value.Second
                };
            case TimeRange.Min30:
            case TimeRange.H1:
                return wd => new
                {
                    wd.Timestamp.Value.Year,
                    wd.Timestamp.Value.Month,
                    wd.Timestamp.Value.Day,
                    wd.Timestamp.Value.Hour,
                    wd.Timestamp.Value.Minute
                };
            case TimeRange.H24:
            case TimeRange.Mo1:
                return wd => new
                {
                    wd.Timestamp.Value.Year,
                    wd.Timestamp.Value.Month,
                    wd.Timestamp.Value.Day,
                    wd.Timestamp.Value.Hour
                };
            case TimeRange.Y1:
                return wd => new
                {
                    wd.Timestamp.Value.Year,
                    wd.Timestamp.Value.Month,
                    wd.Timestamp.Value.Day
                };
            case TimeRange.All:
            default:
                return wd => new
                {
                    wd.Timestamp.Value.Year,
                    wd.Timestamp.Value.Month
                };
        }
    }
}
