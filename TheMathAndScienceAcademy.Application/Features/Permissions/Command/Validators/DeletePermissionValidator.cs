using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Command.Validators;

public class DeletePermissionValidator : AbstractValidator<DeletePermissionCommand>
{
    public DeletePermissionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Permission id is required")
            .NotEqual(Guid.Empty).WithMessage("Permission id must be a valid GUID");
    }
}
