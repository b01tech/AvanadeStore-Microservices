using Inventory.API.Extensions;
using Inventory.API.Middlewares;
using Inventory.Application.Extensions;
using Inventory.Infra.Extensions;
using Prometheus;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .AddUseCases()
    .AddConsumerServices()
    .AddApiDocumentation()
    .AddJwtAuthentication(builder.Configuration)
    .AddMessageBus(builder.Configuration)
    .AddMetrics();

var app = builder.Build();
app.UseMetricServer("/metrics");
app.UseHttpMetrics();
app.MapApiDocumentation();
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.ApplyMigrations();

app.Run();
