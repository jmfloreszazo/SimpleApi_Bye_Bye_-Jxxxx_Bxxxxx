namespace SimpleApi.Pipeline;

/// <summary>
/// A pipeline behavior that wraps handler execution.
/// Replaces MediatR's IPipelineBehavior&lt;TRequest, TResponse&gt;.
/// Behaviors are executed in registration order (outermost first).
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken ct = default);
}
