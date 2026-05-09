using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ProductCatalog.Application.Common.Behaviours;

public sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();
        logger.LogInformation("{Request} completed in {ElapsedMs}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
        return response;
    }
}
