using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleApi.DTO;

namespace SimpleApi.Tests.Integration;

public class UsersEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ═══════════════════════════════════════════════════════════
    // POST /users — Enfoque simple
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task PostUsers_ValidRequest_ShouldReturn201()
    {
        var request = new CreateUserRequest("john@company.com", "John Doe", 30);

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<UserResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.Email.Should().Be("john@company.com");
        body.Name.Should().Be("John Doe");
        body.Age.Should().Be(30);
    }

    [Fact]
    public async Task PostUsers_InvalidEmail_ShouldReturn400()
    {
        var request = new CreateUserRequest("not-an-email", "John Doe", 30);

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostUsers_UnderAge_ShouldReturn400()
    {
        var request = new CreateUserRequest("john@test.com", "John", 10);

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostUsers_ShortName_ShouldReturn400()
    {
        var request = new CreateUserRequest("john@test.com", "AB", 30);

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostUsers_EmailShouldBeTrimmedAndLowered()
    {
        var request = new CreateUserRequest("  JOHN@TEST.COM  ", "John Doe", 30);

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<UserResponse>();
        body!.Email.Should().Be("john@test.com");
    }

    // ═══════════════════════════════════════════════════════════
    // POST /v2/users — Enfoque pipeline (mini-framework)
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task PostV2Users_ValidRequest_ShouldReturn201()
    {
        var request = new CreateUserRequest("jane@company.com", "Jane Doe", 25);

        var response = await _client.PostAsJsonAsync("/v2/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<UserResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.Email.Should().Be("jane@company.com");
        body.Name.Should().Be("Jane Doe");
        body.Age.Should().Be(25);
    }

    [Fact]
    public async Task PostV2Users_InvalidRequest_ShouldReturn400()
    {
        var request = new CreateUserRequest("bad", "A", 5);

        var response = await _client.PostAsJsonAsync("/v2/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV2Users_InvalidEmail_ShouldReturnValidationErrors()
    {
        var request = new CreateUserRequest("not-email", "John Doe", 30);

        var response = await _client.PostAsJsonAsync("/v2/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ═══════════════════════════════════════════════════════════
    // Both endpoints should produce equivalent results
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task BothEndpoints_SameInput_ShouldProduceSameShape()
    {
        var request = new CreateUserRequest("compare@test.com", "Compare User", 22);

        var response1 = await _client.PostAsJsonAsync("/users", request);
        var response2 = await _client.PostAsJsonAsync("/v2/users", request);

        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);

        var body1 = await response1.Content.ReadFromJsonAsync<UserResponse>();
        var body2 = await response2.Content.ReadFromJsonAsync<UserResponse>();

        // Different IDs (different User instances), but same data
        body1!.Id.Should().NotBe(body2!.Id);
        body1.Email.Should().Be(body2.Email);
        body1.Name.Should().Be(body2.Name);
        body1.Age.Should().Be(body2.Age);
    }

    [Fact]
    public async Task BothEndpoints_SameInvalidInput_ShouldBothReturn400()
    {
        var request = new CreateUserRequest("", "AB", 10);

        var response1 = await _client.PostAsJsonAsync("/users", request);
        var response2 = await _client.PostAsJsonAsync("/v2/users", request);

        response1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ═══════════════════════════════════════════════════════════
    // POST /v3/users — Vanilla approach (zero external deps)
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task PostV3Users_ValidRequest_ShouldReturn201()
    {
        var request = new CreateUserRequest("vanilla@company.com", "Vanilla User", 28);

        var response = await _client.PostAsJsonAsync("/v3/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<UserResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.Email.Should().Be("vanilla@company.com");
        body.Name.Should().Be("Vanilla User");
        body.Age.Should().Be(28);
    }

    [Fact]
    public async Task PostV3Users_InvalidEmail_ShouldReturn400()
    {
        var request = new CreateUserRequest("not-an-email", "John Doe", 30);

        var response = await _client.PostAsJsonAsync("/v3/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV3Users_UnderAge_ShouldReturn400()
    {
        var request = new CreateUserRequest("john@test.com", "John", 10);

        var response = await _client.PostAsJsonAsync("/v3/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV3Users_ShortName_ShouldReturn400()
    {
        var request = new CreateUserRequest("john@test.com", "AB", 30);

        var response = await _client.PostAsJsonAsync("/v3/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV3Users_EmailShouldBeTrimmedAndLowered()
    {
        var request = new CreateUserRequest("  VANILLA@TEST.COM  ", "Vanilla User", 30);

        var response = await _client.PostAsJsonAsync("/v3/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<UserResponse>();
        body!.Email.Should().Be("vanilla@test.com");
    }

    // ═══════════════════════════════════════════════════════════
    // POST /v4/users — Vanilla pipeline (zero deps + behaviors)
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task PostV4Users_ValidRequest_ShouldReturn201()
    {
        var request = new CreateUserRequest("v4@company.com", "V4 User", 35);

        var response = await _client.PostAsJsonAsync("/v4/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<UserResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.Email.Should().Be("v4@company.com");
        body.Name.Should().Be("V4 User");
        body.Age.Should().Be(35);
    }

    [Fact]
    public async Task PostV4Users_InvalidRequest_ShouldReturn400()
    {
        var request = new CreateUserRequest("bad", "A", 5);

        var response = await _client.PostAsJsonAsync("/v4/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostV4Users_InvalidEmail_ShouldReturn400()
    {
        var request = new CreateUserRequest("not-email", "John Doe", 30);

        var response = await _client.PostAsJsonAsync("/v4/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ═══════════════════════════════════════════════════════════
    // All 4 endpoints should produce equivalent results
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task AllEndpoints_SameInput_ShouldProduceSameShape()
    {
        var request = new CreateUserRequest("allcheck@test.com", "All Check", 40);

        var r1 = await _client.PostAsJsonAsync("/users", request);
        var r2 = await _client.PostAsJsonAsync("/v2/users", request);
        var r3 = await _client.PostAsJsonAsync("/v3/users", request);
        var r4 = await _client.PostAsJsonAsync("/v4/users", request);

        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        r2.StatusCode.Should().Be(HttpStatusCode.Created);
        r3.StatusCode.Should().Be(HttpStatusCode.Created);
        r4.StatusCode.Should().Be(HttpStatusCode.Created);

        var b1 = await r1.Content.ReadFromJsonAsync<UserResponse>();
        var b2 = await r2.Content.ReadFromJsonAsync<UserResponse>();
        var b3 = await r3.Content.ReadFromJsonAsync<UserResponse>();
        var b4 = await r4.Content.ReadFromJsonAsync<UserResponse>();

        // All different IDs but same data
        var ids = new[] { b1!.Id, b2!.Id, b3!.Id, b4!.Id };
        ids.Should().OnlyHaveUniqueItems();

        b1.Email.Should().Be("allcheck@test.com");
        b2.Email.Should().Be("allcheck@test.com");
        b3.Email.Should().Be("allcheck@test.com");
        b4.Email.Should().Be("allcheck@test.com");
    }

    [Fact]
    public async Task AllEndpoints_SameInvalidInput_ShouldAllReturn400()
    {
        var request = new CreateUserRequest("", "AB", 10);

        var r1 = await _client.PostAsJsonAsync("/users", request);
        var r2 = await _client.PostAsJsonAsync("/v2/users", request);
        var r3 = await _client.PostAsJsonAsync("/v3/users", request);
        var r4 = await _client.PostAsJsonAsync("/v4/users", request);

        r1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        r2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        r3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        r4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
