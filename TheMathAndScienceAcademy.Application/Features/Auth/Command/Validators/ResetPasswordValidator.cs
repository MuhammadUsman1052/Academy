using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Validators;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reset token is required")
            .MaximumLength(128).WithMessage("Reset token must not exceed 128 characters");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters")
            .MaximumLength(128).WithMessage("New password must not exceed 128 characters")
            .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("New password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .MaximumLength(128).WithMessage("Confirm password must not exceed 128 characters")
            .Equal(x => x.NewPassword).WithMessage("Confirm password must match new password");
    }
}