using Auth.API.Extensions;
using Auth.API.Middlewares;
using Auth.Application.Extensions;
using Auth.Infra.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDocumentation()
    .AddUseCases()
    .AddServices()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.MapApiDocumentation();
app.UseExceptionHandlerMiddleware();
app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
