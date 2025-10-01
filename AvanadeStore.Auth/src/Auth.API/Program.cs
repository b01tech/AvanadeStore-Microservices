using Auth.API.Extensions;
using Auth.API.Middlewares;
using Auth.Application.Extensions;
using Auth.Infra.Extensions;
using Prometheus;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDocumentation()
    .AddUseCases()
    .AddServices()
    .AddInfrastructure(builder.Configuration)
    .AddMetrics();

var app = builder.Build();
app.UseMetricServer("/metrics");
app.UseHttpMetrics();
app.MapApiDocumentation();
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.MapEndpoints();
app.ApplyMigrations();

app.Run();
