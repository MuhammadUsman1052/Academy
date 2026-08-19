using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Users.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Users.Command.Validators;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User id is required");
    }
}
