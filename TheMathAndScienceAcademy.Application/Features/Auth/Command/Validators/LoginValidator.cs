using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters")
            .EmailAddress().WithMessage("Email format is invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters");
    }
}
