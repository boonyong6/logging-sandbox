using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Diagnostics.Enrichment;

namespace LoggingSandbox.WebApi.Logging;

public class ActivityCorrelationIdLogEnricher : ILogEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityCorrelationIdLogEnricher(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(IEnrichmentTagCollector collector)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            var httpActivityFeature = httpContext.Features.GetRequiredFeature<IHttpActivityFeature>();

            object? correlationId = httpActivityFeature.Activity.GetTagItem("correlationId");
            if (correlationId is not null)
            {
                collector.Add("correlationId", correlationId);
            }
        }
    }
}
