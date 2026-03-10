using SimpleApi.Domain;
using SimpleApi.DTO;
using SimpleApi.Infrastructure;
using SimpleApi.Pipeline;

namespace SimpleApi.Features.Users;

/// <summary>
/// Handler for CreateUserCommand.
/// Contains mapping + persistence — no AutoMapper, no MediatR.
/// </summary>
public sealed class CreateUserCommandHandler : IHandler<CreateUserCommand, UserResponse>
{
    public Task<UserResponse> HandleAsync(CreateUserCommand request, CancellationToken ct = default)
    {
        // MAP → Domain
        var user = new User(
            request.Email.Trim().ToLower(),
            request.Name.Trim(),
            request.Age);

        // PERSIST
        FakeDatabase.Users.Add(user);

        // MAP → Response
        var response = new UserResponse(
            user.Id,
            user.Email,
            user.Name,
            user.Age);

        return Task.FromResult(response);
    }
}
