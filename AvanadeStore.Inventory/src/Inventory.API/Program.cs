using Inventory.API.Extensions;
using Inventory.API.Middlewares;
using Inventory.Application.Extensions;
using Inventory.Infra.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .AddUseCases()
    .AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
