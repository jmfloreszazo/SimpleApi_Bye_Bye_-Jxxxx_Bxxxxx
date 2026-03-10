using FluentAssertions;
using SimpleApi.DTO;
using SimpleApi.Vanilla;

namespace SimpleApi.Tests.Validation;

public class VanillaValidatorTests
{
    private readonly CreateUserRequestVanillaValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldPass()
    {
        var request = new CreateUserRequest("john@test.com", "John Doe", 30);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
        result.ToDictionary().Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("not-an-email")]
    public void InvalidEmail_ShouldFail(string email)
    {
        var request = new CreateUserRequest(email, "John Doe", 30);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.ToDictionary().Should().ContainKey("Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("AB")]
    public void ShortName_ShouldFail(string name)
    {
        var request = new CreateUserRequest("john@test.com", name, 30);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.ToDictionary().Should().ContainKey("Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(17)]
    public void UnderAge_ShouldFail(int age)
    {
        var request = new CreateUserRequest("john@test.com", "John Doe", age);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.ToDictionary().Should().ContainKey("Age");
    }

    [Fact]
    public void ExactlyAge18_ShouldPass()
    {
        var request = new CreateUserRequest("john@test.com", "John Doe", 18);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void MultipleErrors_ShouldReportAll()
    {
        var request = new CreateUserRequest("", "AB", 10);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.ToDictionary().Should().HaveCount(3);
        result.ToDictionary().Keys.Should().Contain(["Email", "Name", "Age"]);
    }
}
