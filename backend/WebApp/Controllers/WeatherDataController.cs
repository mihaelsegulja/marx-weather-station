using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;
using WebApp.Enums;
using WebApp.Helpers;
using WebApp.Model;

namespace WebApp.Controllers;

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
    public async Task<IActionResult> GetChartData([FromQuery] TimeRange timeRange = TimeRange.Min5)
    {
        try
        {
            var query = _context.WeatherData.AsQueryable();
            query = DataHelpers.ApplyTimeRangeFilter(query, timeRange);

            var groupBySelector = DataHelpers.GetGroupBySelector(timeRange);

            var data = await query
                .Where(wd => wd.Timestamp != null)
                .GroupBy(groupBySelector)
                .Select(g => new WeatherDataDTO
                {
                    Timestamp = g.Max(wd => wd.Timestamp),
                    Iaq = Math.Round(g.Average(wd => wd.Iaq), 2),
                    StaticIaq = Math.Round(g.Average(wd => wd.StaticIaq), 2),
                    Co2equivalent = Math.Round(g.Average(wd => wd.Co2equivalent), 2),
                    BreathVocEquivalent = Math.Round(g.Average(wd => wd.BreathVocEquivalent), 2),
                    CompensatedTemperature = Math.Round(g.Average(wd => wd.CompensatedTemperature), 2),
                    Pressure = Math.Round(g.Average(wd => wd.Pressure), 2),
                    CompensatedHumidity = Math.Round(g.Average(wd => wd.CompensatedHumidity), 2),
                    GasResistance = Math.Round(g.Average(wd => wd.GasResistance), 2)
                })
                .OrderBy(wd => wd.Timestamp)
                .ToListAsync();

            var result = new 
            { 
                Aggregation = timeRange.ToString(), 
                Data = data 
            };

            return Ok(result);
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
    public async Task<IActionResult> GetTableData(
        [FromQuery] TimeRange timeRange = TimeRange.Min5,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.WeatherData.AsQueryable();
            query = DataHelpers.ApplyTimeRangeFilter(query, timeRange);

            var totalCount = await query.CountAsync();

            var data = await query
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
                .ToListAsync();

            var result = new
            {
                Aggregation = timeRange.ToString(),
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
}
