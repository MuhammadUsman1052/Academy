using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Permissions.Query.Models;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Query.Validators;

public class GetPermissionByIdValidator : AbstractValidator<GetPermissionByIdQuery>
{
    public GetPermissionByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Permission id is required")
            .NotEqual(Guid.Empty).WithMessage("Permission id must be a valid GUID");
    }
}
