using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Controllers.DTO;
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

        
        [HttpGet]
        public ActionResult<IEnumerable<WeatherDataDTO>> GetAllWeatherData(int page = 1, int count = 10)
        {
            try
            {
                var weatherDataQuery = _context.WeatherData.Select(w => new WeatherDataDTO
                {
                    Id = w.Id,
                    Iaq = w.Iaq,
                    StaticIaq = w.StaticIaq,
                    Co2equivalent = w.Co2equivalent,
                    BreathVocEquivalent = w.BreathVocEquivalent,
                    CompensatedTemperature = w.CompensatedTemperature,
                    Pressure = w.Pressure,
                    CompensatedHumidity = w.CompensatedHumidity,
                    GasResistance = w.GasResistance
                });

                // Apply pagination
                var pagedWeatherData = weatherDataQuery
                    .Skip((page - 1) * count)
                    .Take(count)           
                    .ToList();

                return Ok(pagedWeatherData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        [HttpGet("{id}")]
        public ActionResult<WeatherDataDTO> GetWeatherDataById(int id)
        {
            try
            {
                var weatherData = _context.WeatherData
                    .Where(w => w.Id == id)
                    .Select(w => new WeatherDataDTO
                    {
                        Id = w.Id,
                        Iaq = w.Iaq,
                        StaticIaq = w.StaticIaq,
                        Co2equivalent = w.Co2equivalent,
                        BreathVocEquivalent = w.BreathVocEquivalent,
                        CompensatedTemperature = w.CompensatedTemperature,
                        Pressure = w.Pressure,
                        CompensatedHumidity = w.CompensatedHumidity,
                        GasResistance = w.GasResistance
                    })
                    .FirstOrDefault();

                if (weatherData == null)
                {
                    return NotFound();
                }

                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
