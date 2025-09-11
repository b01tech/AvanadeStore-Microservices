using Sales.API.Extensions;
using Sales.API.Middlewares;
using Sales.Application.Extensions;
using Sales.Infra.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .AddUseCases()
    .AddApiDocumentation()
    .AddJwtAuthentication(builder.Configuration)
    .AddMessageBus(builder.Configuration);

var app = builder.Build();
app.MapApiDocumentation();
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();
app.ApplyMigrations();

app.Run();
