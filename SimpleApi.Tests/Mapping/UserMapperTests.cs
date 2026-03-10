using FluentAssertions;
using SimpleApi.Domain;
using SimpleApi.DTO;
using SimpleApi.Mapping;

namespace SimpleApi.Tests.Mapping;

public class UserMapperTests
{
    [Fact]
    public void ToDomain_ShouldMapCorrectly()
    {
        var request = new CreateUserRequest("  JOHN@TEST.COM  ", "  John Doe  ", 30);

        var user = UserMapper.ToDomain(request);

        user.Email.Should().Be("john@test.com");    // trimmed + lowered
        user.Name.Should().Be("John Doe");           // trimmed
        user.Age.Should().Be(30);
        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void ToResponse_ShouldMapAllFields()
    {
        var user = new User("john@test.com", "John Doe", 30);

        var response = UserMapper.ToResponse(user);

        response.Id.Should().Be(user.Id);
        response.Email.Should().Be(user.Email);
        response.Name.Should().Be(user.Name);
        response.Age.Should().Be(user.Age);
    }

    [Fact]
    public void ToDomain_ThenToResponse_ShouldRoundTrip()
    {
        var request = new CreateUserRequest("test@example.com", "Alice", 25);

        var user = UserMapper.ToDomain(request);
        var response = UserMapper.ToResponse(user);

        response.Email.Should().Be("test@example.com");
        response.Name.Should().Be("Alice");
        response.Age.Should().Be(25);
    }
}
