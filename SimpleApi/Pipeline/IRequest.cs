namespace SimpleApi.Pipeline;

/// <summary>
/// Marker interface for a request that produces a response of type TResponse.
/// Replaces MediatR's IRequest&lt;TResponse&gt;.
/// </summary>
public interface IRequest<TResponse>;
