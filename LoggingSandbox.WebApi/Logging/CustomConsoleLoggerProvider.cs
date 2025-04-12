using LoggingSandbox.WebApi.Logging.Services;
using Microsoft.Extensions.Logging.Console;

namespace LoggingSandbox.WebApi.Logging;

public class CustomConsoleLoggerProvider : ILoggerProvider
{
    private readonly ConsoleLoggerProvider _innerProvider;
    private readonly ILoggingService _loggingService;

    public CustomConsoleLoggerProvider(
        ConsoleLoggerProvider innerProvider, 
        ICachedLoggingService loggingService)
    {
        _innerProvider = innerProvider;
        _loggingService = loggingService;
    }

    public ILogger CreateLogger(string categoryName)
    {
        ILogger innerLogger = _innerProvider.CreateLogger(categoryName);
        ILogger logger = new CustomLogger(
            categoryName, innerLogger, _loggingService);
        return logger;
    }

    public void Dispose() => _innerProvider.Dispose();
}
