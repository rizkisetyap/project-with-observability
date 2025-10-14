using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Observability.Models;

namespace Observability.Middlewares;

public class TracingHeaderHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<OpenTelemetryOptions> otelOptions
) : DelegatingHandler
{
    private readonly OpenTelemetryOptions _otelOptions = otelOptions.Value;

    /// <summary>
    /// x-request-id: unik per request, dipakai Envoy dan Istio
    /// x-b3-traceid: ID global untuk satu trace chain (semua span) //Panjang ideal: 16 karakter hex (64-bit, sesuai Zipkin standard)
    /// x-b3-spanid: ID unik untuk span saat ini (per hop dalam trace)
    /// x-b3-parentspanid: ID span sebelumnya (opsional, digunakan saat nested span)
    /// x-b3-sampled: "1" artinya trace diaktifkan dan dikirim ke backend tracing (Jaeger, Zipkin, dll)
    /// x-b3-flags: "1" memaksa trace semua request (debug mode), default "0"
    /// x-ot-span-context: format lama OpenTracing, fallback ke GUID string
    /// x-app-source: nama aplikasi yang mengirim request
    /// x-app-version: versi aplikasi yang mengirim request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var context = httpContextAccessor.HttpContext;
        var incoming = context?.Request.Headers;

        var traceId = GetHeaderValue(incoming, "x-b3-traceid") ?? GenerateId(16);
        var parentspanid = GetHeaderValue(incoming, "x-b3-spanid");
        var requestId = GetHeaderValue(incoming, "x-request-id") ?? Guid.NewGuid().ToString();

        var spanId = GenerateId(16);

        // inject header to request
        TrySetHeader(request, "x-request-id", requestId);
        TrySetHeader(request, "x-b3-traceid", traceId);
        TrySetHeader(request, "x-b3-spanid", spanId);
        TrySetHeader(request, "x-b3-sampled", "1");
        TrySetHeader(request, "x-b3-flags", "0");

        if (!string.IsNullOrEmpty(parentspanid))
        {
            TrySetHeader(request, "x-b3-parentspanid", parentspanid);
        }
        TrySetHeader(request, "x-app-source", _otelOptions.ResourceAttributes.ServiceName);
        TrySetHeader(
            request,
            "x-app-version",
            _otelOptions.ResourceAttributes.ServiceVersion ?? "1.0.0"
        );

        return await base.SendAsync(request, cancellationToken);
    }

    private static string? GetHeaderValue(IHeaderDictionary? headers, string key)
    {
        return headers is not null && headers.TryGetValue(key, out var value) && value.Count > 0
            ? value[0]
            : null;
    }

    private static string GenerateId(int length) => Guid.NewGuid().ToString("N")[..length];

    private static void TrySetHeader(HttpRequestMessage request, string key, string value)
    {
        if (!request.Headers.Contains(key))
        {
            request.Headers.TryAddWithoutValidation(key, value);
        }
    }
}
