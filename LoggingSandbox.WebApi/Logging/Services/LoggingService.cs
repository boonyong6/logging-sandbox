using System.Text.Json;

namespace LoggingSandbox.WebApi.Logging.Services;

public class LoggingService : ILoggingService
{
    private readonly IConfiguration _configuration;

    public LoggingService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoggingOptions GetLoggingOptions()
    {
        LoggingOptions loggingOptions = new();
        _configuration.GetSection(LoggingOptions.Logging).Bind(loggingOptions);

        // Populate custom logging options, such as from file, Redis, API, database...
        string loggingOptionsText = File.ReadAllText("loggingsettings.json");
        LoggingOptions customLoggingOptions = 
            JsonSerializer.Deserialize<LoggingOptions>(loggingOptionsText) ?? new();
        
        foreach (KeyValuePair<string, string> logLevel in customLoggingOptions.LogLevel)
        {
            loggingOptions.LogLevel[logLevel.Key]  = logLevel.Value;
        }

        loggingOptions.LogLevel.TryAdd(
            LoggingOptions.DefaultLogLevelKey, LoggingOptions.DefaultLogLevelValue.ToString());

        return loggingOptions;
    }
}
