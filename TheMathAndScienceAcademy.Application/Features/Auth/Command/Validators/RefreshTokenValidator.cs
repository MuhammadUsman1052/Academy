using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required")
            .MaximumLength(256).WithMessage("Refresh token must not exceed 256 characters");
    }
}