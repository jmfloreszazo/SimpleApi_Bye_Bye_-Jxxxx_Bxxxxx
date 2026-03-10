using FluentAssertions;
using SimpleApi.DTO;
using SimpleApi.Validation;

namespace SimpleApi.Tests.Validation;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    [Fact]
    public async Task ValidRequest_ShouldPass()
    {
        var request = new CreateUserRequest("john@test.com", "John Doe", 30);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("not-an-email")]
    public async Task InvalidEmail_ShouldFail(string email)
    {
        var request = new CreateUserRequest(email, "John Doe", 30);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("AB")]
    public async Task ShortName_ShouldFail(string name)
    {
        var request = new CreateUserRequest("john@test.com", name, 30);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(17)]
    public async Task UnderAge_ShouldFail(int age)
    {
        var request = new CreateUserRequest("john@test.com", "John Doe", age);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Age");
    }

    [Fact]
    public async Task ExactlyAge18_ShouldPass()
    {
        var request = new CreateUserRequest("john@test.com", "John Doe", 18);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MultipleErrors_ShouldReportAll()
    {
        var request = new CreateUserRequest("", "AB", 10);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(3);
    }
}
