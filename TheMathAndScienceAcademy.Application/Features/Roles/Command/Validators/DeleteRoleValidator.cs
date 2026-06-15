using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Validators;

public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role id is required")
            .NotEqual(Guid.Empty).WithMessage("Role id must be a valid GUID");
    }
}
