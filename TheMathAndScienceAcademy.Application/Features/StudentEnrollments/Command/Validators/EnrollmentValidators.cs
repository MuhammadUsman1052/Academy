using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Command.Validators;

public class CreateStudentEnrollmentValidator : AbstractValidator<CreateStudentEnrollmentCommand>
{
    public CreateStudentEnrollmentValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty(); RuleFor(x => x.ClassId).NotEmpty(); RuleFor(x => x.AcademicYear).NotEmpty().MaximumLength(30); RuleFor(x => x.EnrollmentDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Enrollment date cannot be in the future.");
    }
}

public class UpdateStudentEnrollmentValidator : AbstractValidator<UpdateStudentEnrollmentCommand>
{
    public UpdateStudentEnrollmentValidator()
    {
        Include(new CreateStudentEnrollmentValidator()); RuleFor(x => x.Id).NotEmpty(); RuleFor(x => x.Status).Must(x => x is "Active" or "Completed" or "Transferred" or "Withdrawn").WithMessage("Invalid enrollment status."); RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.EnrollmentDate).When(x => x.EndDate.HasValue);
    }
}
