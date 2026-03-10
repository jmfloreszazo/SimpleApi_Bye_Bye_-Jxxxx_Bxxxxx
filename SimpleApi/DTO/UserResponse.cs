namespace SimpleApi.DTO;

public record UserResponse(
    Guid Id,
    string Email,
    string Name,
    int Age
);
