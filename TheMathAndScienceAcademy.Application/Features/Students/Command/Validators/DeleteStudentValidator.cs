using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Students.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Validators;

public class DeleteStudentValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentValidator() => RuleFor(x => x.Id).NotEmpty();
}
