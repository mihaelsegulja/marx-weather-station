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
        /// Get all weather data
        /// </summary>
        [HttpGet]
        public IActionResult GetWeatherData()
        {
            try
            {
                var data = _context.WeatherData
                    .ToList()
                    .OrderByDescending(wd => wd.Timestamp);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving weather data: {ex.Message}");
            }
        }
    }
}
