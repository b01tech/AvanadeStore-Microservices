using Gateway.API.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiDocumentation()
    .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapApiDocumentation();

app.Run();
