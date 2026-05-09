using System.Diagnostics;

namespace ProductCatalog.Api.Middleware;

/// <summary>Logs the HTTP method, path, status code, and elapsed ms for every request.</summary>
public sealed class RequestTimingMiddleware(ILogger<RequestTimingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var sw = Stopwatch.StartNew();
        await next(context);
        sw.Stop();

        logger.LogInformation(
            "{Method} {Path} {StatusCode} {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);
    }
}
