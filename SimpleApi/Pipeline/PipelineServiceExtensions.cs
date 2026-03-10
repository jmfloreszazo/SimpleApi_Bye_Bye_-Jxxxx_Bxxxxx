using FluentValidation;
using SimpleApi.Pipeline.Behaviors;
using SimpleApi.Vanilla;

namespace SimpleApi.Pipeline;

/// <summary>
/// Extension methods to wire up the mini-framework in DI.
/// This is the only "registration" code you need — no assembly scanning,
/// no reflection, no magic. ~30 lines.
/// </summary>
public static class PipelineServiceExtensions
{
    /// <summary>
    /// Registers the dispatcher (IDispatcher) as scoped.
    /// </summary>
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        return services;
    }

    /// <summary>
    /// Registers a handler for a specific request/response pair.
    /// </summary>
    public static IServiceCollection AddHandler<TRequest, TResponse, THandler>(
        this IServiceCollection services)
        where TRequest : IRequest<TResponse>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        services.AddScoped<IHandler<TRequest, TResponse>, THandler>();
        return services;
    }

    /// <summary>
    /// Adds the validation behavior for a request/response pair.
    /// </summary>
    public static IServiceCollection AddValidationBehavior<TRequest, TResponse>(
        this IServiceCollection services)
        where TRequest : IRequest<TResponse>
    {
        services.AddScoped<IPipelineBehavior<TRequest, TResponse>,
            ValidationBehavior<TRequest, TResponse>>();
        return services;
    }

    /// <summary>
    /// Adds the logging behavior for a request/response pair.
    /// </summary>
    public static IServiceCollection AddLoggingBehavior<TRequest, TResponse>(
        this IServiceCollection services)
        where TRequest : IRequest<TResponse>
    {
        services.AddScoped<IPipelineBehavior<TRequest, TResponse>,
            LoggingBehavior<TRequest, TResponse>>();
        return services;
    }

    /// <summary>
    /// Adds the VANILLA validation behavior (zero FluentValidation dependency).
    /// </summary>
    public static IServiceCollection AddVanillaValidationBehavior<TRequest, TResponse>(
        this IServiceCollection services)
        where TRequest : IRequest<TResponse>
    {
        services.AddScoped<IPipelineBehavior<TRequest, TResponse>,
            VanillaValidationBehavior<TRequest, TResponse>>();
        return services;
    }
}
