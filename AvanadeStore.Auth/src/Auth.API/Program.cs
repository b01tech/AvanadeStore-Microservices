using Auth.API.Extensions;
using Auth.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDocumentation()
    .AddUseCases();

var app = builder.Build();
app.MapApiDocumentation();
app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
