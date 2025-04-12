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

        LogLevel? configLogLevel = GetConfigLogLevelRecursively(loggingOptions, _categoryName);
        LogLevel minimumLogLevel = configLogLevel ?? GetDefaultLogLevel(loggingOptions);

        return entryLogLevel >= minimumLogLevel;
    }

    private LogLevel? GetConfigLogLevelRecursively(
        LoggingOptions loggingOptions, string categoryName)
    {
        bool hasConfigLogLevel = loggingOptions.LogLevel.TryGetValue(
            categoryName, out string? configLogLevelText);

        if (hasConfigLogLevel)
        {
            if (!Enum.TryParse(configLogLevelText, out LogLevel configLogLevel))
            {
                WriteLine($"[WARN] Failed to parse \"{configLogLevelText}\" to `LogLevel` enum.");
                return null;
            }

            return configLogLevel;
        }

        string? parentNamespace = GetParentNamespace(categoryName);
        if (parentNamespace == null)
        {
            return null;
        }

        return GetConfigLogLevelRecursively(loggingOptions, parentNamespace);
    }

    private string? GetParentNamespace(string namespaceText)
    {
        int lastDotIndex = namespaceText.LastIndexOf('.');
        if (lastDotIndex == -1)
        {
            return null;
        }

        return namespaceText.Substring(0, lastDotIndex);
    }

    private static LogLevel GetDefaultLogLevel(LoggingOptions loggingOptions)
    {
        string defaultLogLevelText = loggingOptions.LogLevel.GetValueOrDefault(
                    LoggingOptions.DefaultLogLevelKey, LoggingOptions.DefaultLogLevelValue.ToString());
        if (!Enum.TryParse(defaultLogLevelText, out LogLevel defaultLogLevel))
        {
            defaultLogLevel = LoggingOptions.DefaultLogLevelValue;
        }

        return defaultLogLevel;
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
