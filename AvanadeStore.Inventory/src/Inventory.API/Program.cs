using Inventory.API.Extensions;
using Inventory.API.Middlewares;
using Inventory.Application.Extensions;
using Inventory.Infra.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .AddUseCases()
    .AddApiDocumentation()
    .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();
app.MapApiDocumentation();
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

app.Run();
