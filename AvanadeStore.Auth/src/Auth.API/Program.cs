using Auth.API.Extensions;
using Auth.API.Middlewares;
using Auth.Application.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDocumentation()
    .AddUseCases()
    .AddServices();

var app = builder.Build();
app.MapApiDocumentation();
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
