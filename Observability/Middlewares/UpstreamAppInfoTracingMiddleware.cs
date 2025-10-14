using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Observability.Middlewares;

public class UpstreamAppInfoTracingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            var appSource = context.Request.Headers["x-app-source"].FirstOrDefault();
            var appVersion = context.Request.Headers["x-app-version"].FirstOrDefault();

            if (!string.IsNullOrEmpty(appSource))
                activity.SetTag("upstream.app.source", appSource);
            if (!string.IsNullOrEmpty(appVersion))
                activity.SetTag("upstream.app.version", appVersion);
        }

        await next(context);
    }
}
