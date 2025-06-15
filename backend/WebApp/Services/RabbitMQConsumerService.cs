using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebApp.Model;
using WebApp.DTO;
using Newtonsoft.Json;

namespace WebApp.Services;

public class RabbitMQConsumerService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQConsumerService> _logger;

    public RabbitMQConsumerService(
        IServiceProvider serviceProvider,
        ILogger<RabbitMQConsumerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = "127.0.0.1",   // Server IP
                Port = 5672,              // Default RabbitMQ port
                UserName = "admin",
                Password = "admin",
                DispatchConsumersAsync = true  // Important for async processing
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the queue
            _channel.QueueDeclare(
                queue: "weather/data",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Bind the queue to the exchange with a routing key
            _channel.QueueBind(
                queue: "weather/data",
                exchange: "amq.topic",
                routingKey: "weather.data");

            _logger.LogInformation("Connected to RabbitMQ");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating RabbitMQ connection");
            throw;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogDebug($"Received message: {message}");

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DbMarxWeatherStationContext>();

                var weatherData = JsonConvert.DeserializeObject<WeatherDataDTO>(message);
                if (weatherData != null)
                {
                    var entity = new WeatherData
                    {
                        Iaq = weatherData.Iaq,
                        StaticIaq = weatherData.StaticIaq,
                        Co2equivalent = weatherData.Co2equivalent,
                        BreathVocEquivalent = weatherData.BreathVocEquivalent,
                        Pressure = weatherData.Pressure,
                        GasResistance = weatherData.GasResistance,
                        CompensatedTemperature = weatherData.CompensatedTemperature,
                        CompensatedHumidity = weatherData.CompensatedHumidity
                    };

                    await dbContext.WeatherData.AddAsync(entity, stoppingToken);
                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogDebug("Data saved to db");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        };

        _channel.BasicConsume(
            queue: "weather/data",
            autoAck: true,
            consumer: consumer
        );

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _channel?.Close();
        _connection?.Close();
        _logger.LogInformation("RabbitMQ connection closed");
    }
}