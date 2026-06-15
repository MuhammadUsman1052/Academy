using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.RolePermissions.Command.Validators;

public class AssignPermissionToRoleValidator : AbstractValidator<AssignPermissionToRoleCommand>
{
    public AssignPermissionToRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required")
            .NotEqual(Guid.Empty).WithMessage("Role id must be a valid GUID");

        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("Permission id is required")
            .NotEqual(Guid.Empty).WithMessage("Permission id must be a valid GUID");
    }
}
