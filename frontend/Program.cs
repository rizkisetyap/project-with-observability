using System.Net.Http.Headers;
using frontend.Service;
using Observability;
using Observability.HealthCheck;
using Observability.Middlewares;
using Observability.Models;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddHttpClient<HttpClientService>(client =>
{
    var apiUrl = builder.Configuration["Backend"];
    if (!string.IsNullOrEmpty(apiUrl))
    {
        client.BaseAddress = new Uri(apiUrl);
    }
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json")
    );
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TracingHeaderHandler>();

builder.AddObservability(otelOptions);
builder.Services.AddHealthCheck(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<UpstreamAppInfoTracingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "frontend", pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseObservability(otelOptions?.Exporters.Metrics);
app.UseHealthCheck();
app.Run();
