using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;  // Added for IServiceScopeFactory
using System.Threading;
using System.Threading.Tasks;
using WebApp.Services;

namespace WebApp.Services
{
    public class MqttBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MqttBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
                using (var scope = _serviceScopeFactory.CreateScope())  // Create a scope for resolving services
                {
                    var mqttService = scope.ServiceProvider.GetRequiredService<MqttService>();  // Get the MqttService from the scope
                    await mqttService.ConnectAndListenAsync();  // Call the method to connect to the MQTT broker
                }

                await Task.Delay(Timeout.Infinite, stoppingToken);
            //}
        }
    }
}
