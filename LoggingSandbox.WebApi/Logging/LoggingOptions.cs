using LogLevelEnum = Microsoft.Extensions.Logging.LogLevel;

namespace LoggingSandbox.WebApi.Logging;

public class LoggingOptions
{
    public const string Logging = "Logging";
    public const string DefaultLogLevelKey = "Default";
    public const LogLevelEnum DefaultLogLevelValue = LogLevelEnum.Information;

    public Dictionary<string, string> LogLevel { get; set; } = new();
}
