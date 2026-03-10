using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleApi.DTO;
using SimpleApi.Features.Users;
using SimpleApi.Pipeline;

namespace SimpleApi.Tests.Pipeline;

/// <summary>
/// Tests that demonstrate how to add custom pipeline behaviors.
/// Shows the extensibility of the mini-framework.
/// </summary>
public class CustomBehaviorTests
{
    /// <summary>
    /// A test behavior that records whether it was called.
    /// Demonstrates how trivial it is to write custom behaviors.
    /// </summary>
    private sealed class TrackingBehavior : IPipelineBehavior<CreateUserCommand, UserResponse>
    {
        public bool WasCalled { get; private set; }
        public CreateUserCommand? ReceivedRequest { get; private set; }

        public async Task<UserResponse> HandleAsync(
            CreateUserCommand request,
            Func<Task<UserResponse>> next,
            CancellationToken ct = default)
        {
            WasCalled = true;
            ReceivedRequest = request;
            return await next();
        }
    }

    /// <summary>
    /// A behavior that modifies the response (e.g., enrichment).
    /// </summary>
    private sealed class ResponseEnrichmentBehavior : IPipelineBehavior<CreateUserCommand, UserResponse>
    {
        public async Task<UserResponse> HandleAsync(
            CreateUserCommand request,
            Func<Task<UserResponse>> next,
            CancellationToken ct = default)
        {
            var response = await next();
            // "Enrich" the response — uppercase the name
            return response with { Name = response.Name.ToUpper() };
        }
    }

    [Fact]
    public async Task CustomBehavior_ShouldBeCalledInPipeline()
    {
        var tracking = new TrackingBehavior();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IHandler<CreateUserCommand, UserResponse>, CreateUserCommandHandler>();
        services.AddSingleton<IPipelineBehavior<CreateUserCommand, UserResponse>>(tracking);

        using var sp = services.BuildServiceProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserCommand("test@test.com", "Test", 20);
        await dispatcher.SendAsync<CreateUserCommand, UserResponse>(command);

        tracking.WasCalled.Should().BeTrue();
        tracking.ReceivedRequest.Should().Be(command);
    }

    [Fact]
    public async Task MultipleBehaviors_ShouldExecuteInOrder()
    {
        var order = new List<string>();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IHandler<CreateUserCommand, UserResponse>, CreateUserCommandHandler>();

        // Register two behaviors — first registered wraps outermost
        services.AddSingleton<IPipelineBehavior<CreateUserCommand, UserResponse>>(
            new OrderTrackingBehavior("First", order));
        services.AddSingleton<IPipelineBehavior<CreateUserCommand, UserResponse>>(
            new OrderTrackingBehavior("Second", order));

        using var sp = services.BuildServiceProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        await dispatcher.SendAsync<CreateUserCommand, UserResponse>(
            new CreateUserCommand("a@b.com", "Test", 20));

        order.Should().ContainInOrder("First:before", "Second:before", "Second:after", "First:after");
    }

    [Fact]
    public async Task EnrichmentBehavior_ShouldModifyResponse()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IHandler<CreateUserCommand, UserResponse>, CreateUserCommandHandler>();
        services.AddSingleton<IPipelineBehavior<CreateUserCommand, UserResponse>>(
            new ResponseEnrichmentBehavior());

        using var sp = services.BuildServiceProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var response = await dispatcher.SendAsync<CreateUserCommand, UserResponse>(
            new CreateUserCommand("a@b.com", "john doe", 20));

        response.Name.Should().Be("JOHN DOE");
    }

    private sealed class OrderTrackingBehavior(string name, List<string> order)
        : IPipelineBehavior<CreateUserCommand, UserResponse>
    {
        public async Task<UserResponse> HandleAsync(
            CreateUserCommand request,
            Func<Task<UserResponse>> next,
            CancellationToken ct = default)
        {
            order.Add($"{name}:before");
            var result = await next();
            order.Add($"{name}:after");
            return result;
        }
    }
}
