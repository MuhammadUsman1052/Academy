using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Command.Validators;

public class CreatePermissionValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required")
            .MaximumLength(100).WithMessage("Permission name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Permission description must not exceed 500 characters");
    }
}
