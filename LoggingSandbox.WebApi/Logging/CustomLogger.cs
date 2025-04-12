using LoggingSandbox.WebApi.Logging.Services;

namespace LoggingSandbox.WebApi.Logging;

public class CustomLogger : ILogger
{
    private readonly string _categoryName;
    private readonly ILogger _innerLogger;
    private readonly ILoggingService _loggingService;

    public CustomLogger(
        string categoryName, 
        ILogger innerLogger, 
        ILoggingService loggingService)
    {
        _categoryName = categoryName;
        _innerLogger = innerLogger;
        _loggingService = loggingService;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _innerLogger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel entryLogLevel)
    {
        LoggingOptions loggingOptions = _loggingService.GetLoggingOptions();

        string defaultLogLevelText = loggingOptions.LogLevel.GetValueOrDefault(
            LoggingOptions.DefaultLogLevelKey, LoggingOptions.DefaultLogLevelValue.ToString());
        if (!Enum.TryParse(defaultLogLevelText, out LogLevel defaultLogLevel))
        {
            defaultLogLevel = LoggingOptions.DefaultLogLevelValue;
        }

        LogLevel configLogLevel = defaultLogLevel;
        bool hasConfigLogLevel = loggingOptions.LogLevel.TryGetValue(
            _categoryName, out string? configLogLevelText) &&
            Enum.TryParse(configLogLevelText, out configLogLevel);

        if (!hasConfigLogLevel)
        {
            return entryLogLevel >= defaultLogLevel;
        }

        return entryLogLevel >= configLogLevel;
    }

    public void Log<TState>(LogLevel entryLogLevel, EventId eventId, TState state, 
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(entryLogLevel))
        {
            return;
        }

        _innerLogger.Log(entryLogLevel, eventId, state, exception, formatter);
    }
}
