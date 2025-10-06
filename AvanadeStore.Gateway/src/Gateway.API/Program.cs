using Gateway.API.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddOcelotConfigurations();

builder.Services.AddApiDocumentation()
    .AddJwtAuthentication(builder.Configuration)
    .AddOcelot();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseHttpMetrics();
app.MapApiDocumentation();
app.UseMetricServer("/metrics");
await app.UseOcelot();

app.Run();
