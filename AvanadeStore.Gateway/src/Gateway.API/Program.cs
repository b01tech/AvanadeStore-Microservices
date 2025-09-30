using Gateway.API.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddOcelotConfigurations();

builder.Services.AddApiDocumentation()
    .AddJwtAuthentication(builder.Configuration)
    .AddOcelot();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapApiDocumentation();
await app.UseOcelot();

app.Run();
