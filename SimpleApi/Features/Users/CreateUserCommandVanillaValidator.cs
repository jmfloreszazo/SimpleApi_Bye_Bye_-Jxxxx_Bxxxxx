using SimpleApi.Vanilla;

namespace SimpleApi.Features.Users;

/// <summary>
/// Vanilla validator for the pipeline endpoint — zero external dependencies.
/// </summary>
public sealed class CreateUserCommandVanillaValidator : IVanillaValidator<CreateUserVanillaCommand>
{
    public VanillaValidationResult Validate(CreateUserVanillaCommand r)
    {
        var result = new VanillaValidationResult();

        if (string.IsNullOrWhiteSpace(r.Email) || !r.Email.Contains('@'))
            result.AddError(nameof(r.Email), "Email must be a valid email address.");

        if (string.IsNullOrWhiteSpace(r.Name) || r.Name.Trim().Length < 3)
            result.AddError(nameof(r.Name), "Name must be at least 3 characters long.");

        if (r.Age < 18)
            result.AddError(nameof(r.Age), "Age must be greater than or equal to 18.");

        return result;
    }
}
