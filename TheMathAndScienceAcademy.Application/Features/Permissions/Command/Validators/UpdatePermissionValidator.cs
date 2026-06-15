using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Command.Validators;

public class UpdatePermissionValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Permission id is required")
            .NotEqual(Guid.Empty).WithMessage("Permission id must be a valid GUID");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required")
            .MaximumLength(100).WithMessage("Permission name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Permission description must not exceed 500 characters");
    }
}
