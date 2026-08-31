using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Students.Query.Models;

namespace TheMathAndScienceAcademy.Application.Features.Students.Query.Validators;

public class GetStudentByIdValidator : AbstractValidator<GetStudentByIdQuery>
{
    public GetStudentByIdValidator() => RuleFor(x => x.Id).NotEmpty();
}
