using Microsoft.EntityFrameworkCore;
using WebApp.Model;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
