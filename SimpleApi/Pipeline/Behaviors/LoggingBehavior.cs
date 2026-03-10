using Microsoft.Extensions.Logging;

namespace SimpleApi.Pipeline.Behaviors;

/// <summary>
/// Pipeline behavior that logs request/response and timing.
/// Demonstrates how easy it is to add cross-cutting concerns.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken ct = default)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("[Pipeline] Handling {Request}", requestName);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        logger.LogInformation(
            "[Pipeline] Handled {Request} in {ElapsedMs}ms",
            requestName, sw.ElapsedMilliseconds);

        return response;
    }
}
