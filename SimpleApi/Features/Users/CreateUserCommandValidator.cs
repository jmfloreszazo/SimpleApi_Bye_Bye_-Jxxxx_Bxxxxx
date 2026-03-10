using FluentValidation;

namespace SimpleApi.Features.Users;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
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
