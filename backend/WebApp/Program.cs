using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApp.Model;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });
    c.UseInlineDefinitionsForEnums();
});
builder.Services.AddDbContext<DbMarxWeatherStationContext>(options =>
{
    options.UseSqlServer("Name=ConnectionStrings:Default");
});

builder.Services.AddHostedService<RabbitMQConsumerService>();

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
