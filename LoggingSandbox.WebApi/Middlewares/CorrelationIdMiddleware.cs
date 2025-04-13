using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace LoggingSandbox.WebApi.Middlewares;

public class CorrelationIdMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? correlationId = null;
        
        if (context.Request.Headers.TryGetValue("X-Correlation-Id", out StringValues values))
        {
            correlationId = values.First();
        }

        correlationId ??= Guid.NewGuid().ToString();

        var activityFeature = context.Features.GetRequiredFeature<IHttpActivityFeature>();
        activityFeature.Activity.SetTag("correlationId", correlationId);

        context.Response.Headers["X-Correlation-Id"] = correlationId;

        await next(context);
    }
}
