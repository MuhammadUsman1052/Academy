using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Validators;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
