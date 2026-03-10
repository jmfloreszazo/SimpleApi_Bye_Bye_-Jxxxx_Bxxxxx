namespace SimpleApi.Pipeline;

/// <summary>
/// Dispatches requests through the pipeline (behaviors → handler).
/// Replaces MediatR's IMediator / ISender.
/// Zero reflection — the DI container resolves everything at compile-known types.
/// </summary>
public interface IDispatcher
{
    Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct = default)
        where TRequest : IRequest<TResponse>;
}

public sealed class Dispatcher(IServiceProvider sp) : IDispatcher
{
    public Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct = default)
        where TRequest : IRequest<TResponse>
    {
        // Resolve the actual handler
        var handler = sp.GetRequiredService<IHandler<TRequest, TResponse>>();

        // Resolve all pipeline behaviors (ordered by registration)
        var behaviors = sp.GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()  // reverse so the first registered wraps outermost
            .ToList();

        // Build the pipeline: innermost = handler, each behavior wraps the next
        Func<Task<TResponse>> pipeline = () => handler.HandleAsync(request, ct);

        foreach (var behavior in behaviors)
        {
            var next = pipeline; // capture for closure
            pipeline = () => behavior.HandleAsync(request, next, ct);
        }

        return pipeline();
    }
}
