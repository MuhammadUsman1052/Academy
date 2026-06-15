using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Validators;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role id is required")
            .NotEqual(Guid.Empty).WithMessage("Role id must be a valid GUID");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required")
            .MinimumLength(2).WithMessage("Role name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Role name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Role description must not exceed 500 characters");
    }
}
