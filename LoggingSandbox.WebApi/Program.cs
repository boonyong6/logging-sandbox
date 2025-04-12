using LoggingSandbox.WebApi.Logging;
using LoggingSandbox.WebApi.Logging.Services;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Services.AddSingleton<ConsoleLoggerProvider>();
builder.Services.AddSingleton<ILoggerProvider, CustomConsoleLoggerProvider>();
builder.Services.AddSingleton<ILoggingService, LoggingService>();
builder.Services.AddSingleton<ICachedLoggingService, CachedLoggingService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
