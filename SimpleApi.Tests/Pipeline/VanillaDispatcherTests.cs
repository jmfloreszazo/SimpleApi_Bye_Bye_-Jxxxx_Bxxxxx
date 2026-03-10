using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleApi.DTO;
using SimpleApi.Features.Users;
using SimpleApi.Pipeline;
using SimpleApi.Vanilla;

namespace SimpleApi.Tests.Pipeline;

public class VanillaDispatcherTests
{
    private static ServiceProvider BuildProvider(bool withValidation = true, bool withLogging = false)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        // Vanilla validators
        services.AddScoped<IVanillaValidator<CreateUserVanillaCommand>, CreateUserCommandVanillaValidator>();

        // Pipeline
        services.AddDispatcher();
        services.AddHandler<CreateUserVanillaCommand, UserResponse, CreateUserVanillaCommandHandler>();

        if (withValidation)
            services.AddVanillaValidationBehavior<CreateUserVanillaCommand, UserResponse>();

        if (withLogging)
            services.AddLoggingBehavior<CreateUserVanillaCommand, UserResponse>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Send_ValidCommand_ShouldReturnResponse()
    {
        using var sp = BuildProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserVanillaCommand("john@test.com", "John Doe", 30);

        var response = await dispatcher.SendAsync<CreateUserVanillaCommand, UserResponse>(command);

        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
        response.Email.Should().Be("john@test.com");
        response.Name.Should().Be("John Doe");
        response.Age.Should().Be(30);
    }

    [Fact]
    public async Task Send_InvalidCommand_ShouldThrowVanillaValidationException()
    {
        using var sp = BuildProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserVanillaCommand("not-email", "AB", 10);

        var act = () => dispatcher.SendAsync<CreateUserVanillaCommand, UserResponse>(command);

        await act.Should().ThrowAsync<VanillaValidationException>()
            .Where(ex => ex.Errors.Count > 0);
    }

    [Fact]
    public async Task Send_WithLogging_ShouldStillReturnResponse()
    {
        using var sp = BuildProvider(withValidation: true, withLogging: true);
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserVanillaCommand("jane@test.com", "Jane Doe", 25);

        var response = await dispatcher.SendAsync<CreateUserVanillaCommand, UserResponse>(command);

        response.Email.Should().Be("jane@test.com");
    }

    [Fact]
    public async Task Send_WithoutValidationBehavior_ShouldSkipValidation()
    {
        using var sp = BuildProvider(withValidation: false);
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserVanillaCommand("bad", "A", 5);

        var act = () => dispatcher.SendAsync<CreateUserVanillaCommand, UserResponse>(command);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task VanillaValidation_ShouldReturnAllErrors()
    {
        using var sp = BuildProvider();
        var dispatcher = sp.GetRequiredService<IDispatcher>();

        var command = new CreateUserVanillaCommand("bad", "AB", 10);

        var act = () => dispatcher.SendAsync<CreateUserVanillaCommand, UserResponse>(command);

        var ex = (await act.Should().ThrowAsync<VanillaValidationException>()).Which;
        ex.Errors.Should().ContainKey("Email");
        ex.Errors.Should().ContainKey("Name");
        ex.Errors.Should().ContainKey("Age");
    }
}
