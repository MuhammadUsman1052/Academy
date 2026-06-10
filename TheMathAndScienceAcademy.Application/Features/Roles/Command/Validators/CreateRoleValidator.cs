using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Validators;

public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
