using SimpleApi.Domain;
using SimpleApi.DTO;
using SimpleApi.Infrastructure;
using SimpleApi.Pipeline;

namespace SimpleApi.Features.Users;

/// <summary>
/// Handler for the fully vanilla pipeline — zero external dependencies.
/// </summary>
public sealed class CreateUserVanillaCommandHandler : IHandler<CreateUserVanillaCommand, UserResponse>
{
    public Task<UserResponse> HandleAsync(CreateUserVanillaCommand request, CancellationToken ct = default)
    {
        var user = new User(
            request.Email.Trim().ToLower(),
            request.Name.Trim(),
            request.Age);

        FakeDatabase.Users.Add(user);

        var response = new UserResponse(user.Id, user.Email, user.Name, user.Age);
        return Task.FromResult(response);
    }
}
