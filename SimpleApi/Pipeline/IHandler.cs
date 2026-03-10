namespace SimpleApi.Pipeline;

/// <summary>
/// Handles a request and returns a response.
/// Replaces MediatR's IRequestHandler&lt;TRequest, TResponse&gt;.
/// </summary>
public interface IHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}
