using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Students.Query.Models;

namespace TheMathAndScienceAcademy.Application.Features.Students.Query.Validators;

public class GetStudentsByAcademyValidator : AbstractValidator<GetStudentsByAcademyQuery>
{
    public GetStudentsByAcademyValidator() => RuleFor(x => x.AcademyId).NotEmpty();
}
