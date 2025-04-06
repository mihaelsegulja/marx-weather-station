using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttSubscriberController : ControllerBase
    {
        private readonly MqttService _mqttService;

        // Inject the MqttService dependency
        public MqttSubscriberController(MqttService mqttService)
        {
            _mqttService = mqttService;
        }

        // POST api/MqttSubscriber/start
        [HttpPost("start")]
        public IActionResult StartListening()
        {
            try
            {
                // Start listening for MQTT messages asynchronously
                _mqttService?.ConnectAndListenAsync();

                // Return a success message
                return Ok("Started listening for MQTT messages.");
            }
            catch (Exception ex)
            {
                // Return an error message if something goes wrong
                return StatusCode(500, $"Failed to start listening: {ex.Message}");
            }
        }
    }
}
