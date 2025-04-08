using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Text;
using WebApp.Model;

namespace WebApp.Services
{
    public class MqttService
    {
        private IMqttClient _mqttClient;
        private readonly DbMarxWeatherStationContext _context;
        private const string MqttBrokerAddress = "127.0.0.1";
        private const string MqttTopic = "weather/data";  // Topic to which the ESP32 device sends data

        private bool _isConnected = false; // Prevents multiple connections to the broker
        public MqttService(DbMarxWeatherStationContext context)
        {
            _context = context;
        }

        // This method connects to the MQTT broker and listens for messages asynchronously
        public async Task ConnectAndListenAsync()  // Changed to async and Task return type
        {
            if (_isConnected) return; // If already connected, do not reconnect

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithCredentials("admin", "admin")
                .WithTcpServer(MqttBrokerAddress)  // Setting the address of the MQTT broker
                .Build();

            // Handler for when connection is established
            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Connected to MQTT broker");  // Log when connected
                await _mqttClient.SubscribeAsync(MqttTopic);  // Asynchronously subscribe to the topic
            };

            // Handler for when a message is received on the subscribed topic
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);  // Get the message payload
                Console.WriteLine($"Received message: {payload}");  // Log the received message

                // Deserialize the message to a WeatherData object
                var weatherData = JsonConvert.DeserializeObject<WeatherData>(payload);

                // Save the received data to the database
                SaveWeatherDataToDatabase(weatherData);
            };

            try
            {
                await _mqttClient.ConnectAsync(options);  // Connect to the MQTT broker asynchronously
                _isConnected = true; // Set flag to true after successful connection
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {ex.Message}");  // Log the error if connection fails
            }
        }

        // This method saves the received weather data to the database
        private void SaveWeatherDataToDatabase(WeatherData weatherData)
        {
            if (weatherData != null)
            {
                _context.WeatherData.Add(weatherData);  // Add the new data to the database
                _context.SaveChanges();  // Save changes to the database 
            }
        }
    }
}
