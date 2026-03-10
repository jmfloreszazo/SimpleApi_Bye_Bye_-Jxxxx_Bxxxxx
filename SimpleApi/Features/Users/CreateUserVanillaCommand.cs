using SimpleApi.DTO;
using SimpleApi.Pipeline;

namespace SimpleApi.Features.Users;

/// <summary>
/// Command for the fully vanilla pipeline (v4).
/// Same shape as CreateUserCommand but with its own type
/// so the pipeline behaviors are registered independently.
/// </summary>
public record CreateUserVanillaCommand(string Email, string Name, int Age) : IRequest<UserResponse>;
