using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Validators;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters")
            .EmailAddress().WithMessage("Email format is invalid");
    }
}