using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Users.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Users.Command.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User name is required")
            .MinimumLength(2).WithMessage("User name must be at least 2 characters")
            .MaximumLength(200).WithMessage("User name must not exceed 200 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required");

        RuleFor(x => x.AcademyId)
            .Must(x => x is null || x != Guid.Empty)
            .WithMessage("Academy id must be a valid GUID");
    }
}
