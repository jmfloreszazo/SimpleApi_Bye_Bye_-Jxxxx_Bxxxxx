using SimpleApi.DTO;
using SimpleApi.Domain;

namespace SimpleApi.Mapping;

public static class UserMapper
{
    public static User ToDomain(CreateUserRequest dto)
    {
        return new User(
            dto.Email.Trim().ToLower(),
            dto.Name.Trim(),
            dto.Age
        );
    }

    public static UserResponse ToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.Email,
            user.Name,
            user.Age
        );
    }
}
