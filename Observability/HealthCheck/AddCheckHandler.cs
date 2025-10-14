// -----------------------------------------------------------------------------------
// Copyright (c) 2025 rizkisetyap. All rights reserved.
// Author: rizkisetyap (https://github.com/rizkisetyap)
// Project: ObservabilityTools
// -----------------------------------------------------------------------------------

namespace Observability.HealthCheck;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class AddCheckHandler
{
    public static void AddHealthCheck(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready", "live" });
    }

    public static void UseHealthCheck(this IApplicationBuilder app)
    {
        app.UseHealthChecks(
            "/health/ready",
            new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = UiResponseWriter.WriteMinimalResponse,
            }
        );
        app.UseHealthChecks(
            "/health/live",
            new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live"),
                ResponseWriter = UiResponseWriter.WriteMinimalResponse,
            }
        );
    }
}
