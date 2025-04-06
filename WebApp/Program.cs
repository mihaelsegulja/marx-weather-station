using WebApp.Model;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MqttService as Scoped, because it depends on DbContext
builder.Services.AddScoped<MqttService>();

// Register MqttBackgroundService as Transient (not Singleton)
builder.Services.AddTransient<IHostedService, MqttBackgroundService>();  // Use Transient, not Scoped or Singleton

// Register other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DbMarxWeatherStationContext>();

var app = builder.Build();

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
