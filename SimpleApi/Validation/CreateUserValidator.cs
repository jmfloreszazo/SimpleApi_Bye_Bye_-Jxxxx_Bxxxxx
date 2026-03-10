using FluentValidation;
using SimpleApi.DTO;

namespace SimpleApi.Validation;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Name)
            .MinimumLength(3);

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18);
    }
}
