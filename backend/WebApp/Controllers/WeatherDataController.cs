    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WebApp.DTO;
    using WebApp.Model;

    namespace WebApp.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class WeatherDataController : ControllerBase
        {
            private readonly DbMarxWeatherStationContext _context;

            public WeatherDataController(DbMarxWeatherStationContext context)
            {
                _context = context;
            }


            /// <summary>
            /// Get chart data for a specific time range
            /// </summary>
            [HttpGet("chart")]
            public IActionResult GetChartData([FromQuery] string timeRange = "5min")
            {
                try
                {
                    var query = _context.WeatherData.AsQueryable();

                    // Apply time range filter
                    query = ApplyTimeRangeFilter(query, timeRange);

                    // For charts, we return all data points within the range, ordered by timestamp
                    var data = query
                        .OrderBy(wd => wd.Timestamp)
                        .Select(wd => new WeatherDataDTO
                        {
                            Id = wd.Id,
                            Timestamp = wd.Timestamp,
                            Iaq = wd.Iaq,
                            StaticIaq = wd.StaticIaq,
                            Co2equivalent = wd.Co2equivalent,
                            BreathVocEquivalent = wd.BreathVocEquivalent,
                            CompensatedTemperature = wd.CompensatedTemperature,
                            Pressure = wd.Pressure,
                            CompensatedHumidity = wd.CompensatedHumidity,
                            GasResistance = wd.GasResistance
                        })
                        .ToList();

                    return Ok(data);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error retrieving chart data: {ex.Message}");
                }
            }

            /// <summary>
            /// Get paginated table data for a specific time range
            /// </summary>
            [HttpGet("table")]
            public IActionResult GetTableData(
                [FromQuery] string timeRange = "5min",
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
            {
                try
                {
                    var query = _context.WeatherData.AsQueryable();

                    // Apply time range filter
                    query = ApplyTimeRangeFilter(query, timeRange);

                    // Get total count for pagination info
                    var totalCount = query.Count();

                    // Apply pagination
                    var data = query
                        .OrderByDescending(wd => wd.Timestamp)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(wd => new WeatherDataDTO
                        {
                            Id = wd.Id,
                            Timestamp = wd.Timestamp,
                            Iaq = wd.Iaq,
                            StaticIaq = wd.StaticIaq,
                            Co2equivalent = wd.Co2equivalent,
                            BreathVocEquivalent = wd.BreathVocEquivalent,
                            CompensatedTemperature = wd.CompensatedTemperature,
                            Pressure = wd.Pressure,
                            CompensatedHumidity = wd.CompensatedHumidity,
                            GasResistance = wd.GasResistance
                        })
                        .ToList();

                    var result = new
                    {
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                        CurrentPage = page,
                        PageSize = pageSize,
                        Data = data
                    };

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error retrieving table data: {ex.Message}");
                }
            }

            // Helper method to apply time range filtering
            private IQueryable<WeatherData> ApplyTimeRangeFilter(IQueryable<WeatherData> query, string timeRange)
            {
                DateTime cutoffTime = DateTime.UtcNow;

                switch (timeRange.ToLower())
                {
                    case "5min":
                        cutoffTime = DateTime.UtcNow.AddMinutes(-5);
                        break;
                    case "10min":
                        cutoffTime = DateTime.UtcNow.AddMinutes(-10);
                        break;
                    case "30min":
                        cutoffTime = DateTime.UtcNow.AddMinutes(-30);
                        break;
                    case "1h":
                        cutoffTime = DateTime.UtcNow.AddHours(-1);
                        break;
                    case "24h":
                        cutoffTime = DateTime.UtcNow.AddHours(-24);
                        break;
                    case "1mo":
                        cutoffTime = DateTime.UtcNow.AddMonths(-1);
                        break;
                    case "1y":
                        cutoffTime = DateTime.UtcNow.AddYears(-1);
                        break;
                    case "all":
                    default:
                        return query;
                }

                return query.Where(wd => wd.Timestamp >= cutoffTime);
            }
        }
    }

