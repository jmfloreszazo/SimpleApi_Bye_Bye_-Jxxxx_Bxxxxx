using FluentAssertions;
using SimpleApi.Domain;

namespace SimpleApi.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var user = new User("john@test.com", "John Doe", 30);

        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be("john@test.com");
        user.Name.Should().Be("John Doe");
        user.Age.Should().Be(30);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        var user1 = new User("a@b.com", "A", 20);
        var user2 = new User("c@d.com", "B", 25);

        user1.Id.Should().NotBe(user2.Id);
    }
}
