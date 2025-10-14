namespace Observability.Models;

public class OpenTelemetryOptions
{
    public ResourceAttributeOptions ResourceAttributes { get; set; } = new();
    public ExporterOptions Exporters { get; set; } = new();
    public OtlpOptions Otlp { get; set; } = new();
}

public class ResourceAttributeOptions
{
    public bool IncludeFormattedMessage { get; set; } = true;
    public bool IncludeScopes { get; set; } = true;
    public bool ParseStateValues { get; set; } = true;
    public string ServiceName { get; set; }
    public string? ServiceVersion { get; set; }
    public string? NameSpace { get; set; }
    public string ServiceInstanceId { get; set; }

    public ResourceAttributeOptions(
        string serviceInstanceId,
        string service,
        string serviceName,
        string? serviceVersion
    )
    {
        ServiceInstanceId = serviceInstanceId;
        ServiceName = serviceName;
        ServiceVersion = serviceVersion;
    }

    public ResourceAttributeOptions()
    {
        ServiceName = string.Empty;
        ServiceInstanceId = string.Empty;
    }
}

public class ExporterOptions
{
    public string? Traces { get; set; } = "none";
    public string? Metrics { get; set; } = "none";
    public string? Logs { get; set; } = "none";
}

public class OtlpOptions
{
    public string Endpoint { get; set; }

    public OtlpOptions(string endpoint)
    {
        Endpoint = endpoint;
    }

    public OtlpOptions()
    {
        Endpoint = string.Empty;
    }
}
