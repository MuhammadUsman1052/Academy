using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Command.Validators;

public class DeleteAcademyValidator : AbstractValidator<DeleteAcademyCommand>
{
    public DeleteAcademyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Academy id is required")
            .NotEqual(Guid.Empty).WithMessage("Academy id must be a valid GUID");
    }
}