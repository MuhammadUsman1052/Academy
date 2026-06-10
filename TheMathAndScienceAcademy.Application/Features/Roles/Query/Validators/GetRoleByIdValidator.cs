using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Roles.Query.Models;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Query.Validators;

public class GetRoleByIdValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
