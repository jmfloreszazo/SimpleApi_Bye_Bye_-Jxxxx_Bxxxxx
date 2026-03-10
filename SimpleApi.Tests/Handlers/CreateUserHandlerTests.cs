using FluentAssertions;
using SimpleApi.Domain;
using SimpleApi.Handlers;
using SimpleApi.Infrastructure;

namespace SimpleApi.Tests.Handlers;

public class CreateUserHandlerTests
{
    [Fact]
    public void Handle_ShouldAddUserToDatabase()
    {
        var handler = new CreateUserHandler();
        var user = new User("test@test.com", "Test", 20);
        var initialCount = FakeDatabase.Users.Count;

        var result = handler.Handle(user);

        result.Should().BeSameAs(user);
        FakeDatabase.Users.Count.Should().Be(initialCount + 1);
        FakeDatabase.Users.Should().Contain(user);
    }
}
