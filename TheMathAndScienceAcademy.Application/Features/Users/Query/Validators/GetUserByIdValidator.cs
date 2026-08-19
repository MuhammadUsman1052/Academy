using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Users.Query.Models;

namespace TheMathAndScienceAcademy.Application.Features.Users.Query.Validators;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User id is required");
    }
}
