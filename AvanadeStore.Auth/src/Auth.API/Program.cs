using Auth.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDocumentation();

var app = builder.Build();
app.MapApiDocumentation();
app.UseHttpsRedirection();
app.Run();
