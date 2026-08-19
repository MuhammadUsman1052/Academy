using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Users.Query.Models;

namespace TheMathAndScienceAcademy.Application.Features.Users.Query.Validators;

public class GetUsersByAcademyValidator : AbstractValidator<GetUsersByAcademyQuery>
{
    public GetUsersByAcademyValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy id is required");
    }
}
