using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Observability.HealthCheck;

public static class UiResponseWriter
{
    private static readonly Lazy<JsonSerializerOptions> Options = new(CreateJsonOptions);

    public static async Task WriteMinimalResponse(
        HttpContext httpContext,
        HealthReport healthReport
    )
    {
        httpContext.Response.ContentType = "application/json";
        var result = new
        {
            status = healthReport.Status == HealthStatus.Healthy ? "ready" : "not ready",
            details = healthReport.Entries.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.Status == HealthStatus.Healthy ? "up" : "down"
            ),
        };

        if (healthReport.Status != HealthStatus.Healthy)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        await JsonSerializer.SerializeAsync(httpContext.Response.Body, result, Options.Value);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new TimeSpanConverter());

        return options;
    }
}
