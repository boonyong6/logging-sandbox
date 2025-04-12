using Microsoft.Extensions.Caching.Memory;

namespace LoggingSandbox.WebApi.Logging.Services;

public class CachedLoggingService : ICachedLoggingService
{
    private readonly IServiceProvider _provider;
    private readonly ILoggingService _innerService;
    private IMemoryCache? _cache;

    public CachedLoggingService(
        ILoggingService loggingService, 
        IServiceProvider provider)
    {
        _innerService = loggingService;
        _provider = provider;
    }

    private IMemoryCache Cache => _cache ??= 
        _provider.GetRequiredService<IMemoryCache>();

    public LoggingOptions GetLoggingOptions()
    {
        string cacheKey = "loggingOptions";
        if (Cache.TryGetValue(cacheKey, out LoggingOptions? cachedLoggingOptions))
        {
            return cachedLoggingOptions ?? new();
        }

        LoggingOptions loggingOptions = Cache.Set(
            cacheKey, _innerService.GetLoggingOptions(), TimeSpan.FromSeconds(10));

        return loggingOptions;
    }
}
