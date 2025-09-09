using Auth.API.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiDocumentation();

var app = builder.Build();
app.MapApiDocumentation();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
