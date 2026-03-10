using SimpleApi.DTO;
using SimpleApi.Pipeline;

namespace SimpleApi.Features.Users;

/// <summary>
/// Command that flows through the pipeline.
/// Implements IRequest&lt;TResponse&gt; from our mini-framework.
/// </summary>
public record CreateUserCommand(
    string Email,
    string Name,
    int Age
) : IRequest<UserResponse>;
