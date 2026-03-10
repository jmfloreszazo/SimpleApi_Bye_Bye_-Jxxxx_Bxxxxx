namespace SimpleApi.DTO;

public record CreateUserRequest(
    string Email,
    string Name,
    int Age
);
