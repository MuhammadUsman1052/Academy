using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Query.Models;

namespace TheMathAndScienceAcademy.Application.Features.RolePermissions.Query.Validators;

public class GetRolePermissionsValidator : AbstractValidator<GetRolePermissionsQuery>
{
    public GetRolePermissionsValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role id is required")
            .NotEqual(Guid.Empty).WithMessage("Role id must be a valid GUID");
    }
}
