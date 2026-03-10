using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimpleApi.DTO;
using SimpleApi.Features.Users;
using SimpleApi.Infrastructure;
using SimpleApi.Pipeline;
using SimpleApi.Pipeline.Behaviors;

namespace SimpleApi.Tests.Pipeline;

public class DispatcherTests
{
    private static ServiceProvider BuildProvider(bool withValidation = true, bool withLogging = false)
    {
        var services = new ServiceCollection();

        // Logging (needed by LoggingBehavior)
        services.AddLogging();

        // FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

        // Pipeline
        services.AddDispatcher();
        services.AddHandler<CreateUserCommand, UserResponse, CreateUserCommandHandler>();

        if (withValidation)
            services.AddValidationBehavior<CreateUserCommand, UserResponse>();

        if (withLogging)
            services.AddLoggingBehavior<CreateUserCommand, UserResponse>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Send_ValidCommand_ShouldReturnResponse()
    {
        using var sp = BuildProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserCommand("john@test.com", "John Doe", 30);

        var response = await dispatcher.SendAsync<CreateUserCommand, UserResponse>(command);

        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
        response.Email.Should().Be("john@test.com");
        response.Name.Should().Be("John Doe");
        response.Age.Should().Be(30);
    }

    [Fact]
    public async Task Send_InvalidCommand_ShouldThrowValidationException()
    {
        using var sp = BuildProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserCommand("not-email", "AB", 10);

        var act = () => dispatcher.SendAsync<CreateUserCommand, UserResponse>(command);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any());
    }

    [Fact]
    public async Task Send_WithLogging_ShouldStillReturnResponse()
    {
        using var sp = BuildProvider(withValidation: true, withLogging: true);
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserCommand("jane@test.com", "Jane Doe", 25);

        var response = await dispatcher.SendAsync<CreateUserCommand, UserResponse>(command);

        response.Email.Should().Be("jane@test.com");
    }

    [Fact]
    public async Task Send_WithoutValidationBehavior_ShouldSkipValidation()
    {
        using var sp = BuildProvider(withValidation: false);
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        // This would fail validation, but we disabled the behavior
        var command = new CreateUserCommand("bad", "A", 5);

        var act = () => dispatcher.SendAsync<CreateUserCommand, UserResponse>(command);

        // Should NOT throw — no validation behavior registered
        await act.Should().NotThrowAsync();
    }
}
