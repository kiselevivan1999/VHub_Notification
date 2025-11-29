using Infrastructure.EntityFramework;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("emailSettings.json");

services.AddServices(configuration);
services.AddControllers();
services.AddSwaggerGen();

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

var app = builder.Build();
await app.Services.MigrateDatabase();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
