using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Validators;

public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
