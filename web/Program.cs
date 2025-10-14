using Microsoft.EntityFrameworkCore;
using Observability;
using Observability.HealthCheck;
using Observability.Middlewares;
using Observability.Models;
using web.Domain.Data;

var builder = WebApplication.CreateBuilder(args);

// Add appsettings
builder.Configuration.AddJsonFile("appsetting.json", true, true);
builder.Configuration.AddJsonFile(
    $"appsetting.{builder.Environment.EnvironmentName}.json",
    true,
    false
);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);
builder.Services.Configure<OpenTelemetryOptions>(
    builder.Configuration.GetSection("OpenTelemetryOptions")
);
var otelOptions = builder
    .Configuration.GetSection("OpenTelemetryOptions")
    .Get<OpenTelemetryOptions>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TracingHeaderHandler>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.AddObservability(otelOptions);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthCheck(builder.Configuration);
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<UpstreamAppInfoTracingMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.UseObservability(otelOptions?.Exporters.Metrics);
app.UseHealthCheck();
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
