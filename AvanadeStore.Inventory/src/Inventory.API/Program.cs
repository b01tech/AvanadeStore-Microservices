using Inventory.API.Extensions;
using Inventory.API.Middlewares;
using Inventory.Application.Extensions;
using Inventory.Infra.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .AddUseCases()
    .AddApiDocumentation();

var app = builder.Build();
app.MapApiDocumentation();
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
