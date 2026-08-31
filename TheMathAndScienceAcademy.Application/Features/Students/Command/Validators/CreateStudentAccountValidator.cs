using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Students.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Validators;

public class CreateStudentAccountValidator : AbstractValidator<CreateStudentAccountCommand>
{
    public CreateStudentAccountValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student id is required.");
    }
}
