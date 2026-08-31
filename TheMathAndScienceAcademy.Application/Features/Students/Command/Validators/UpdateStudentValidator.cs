using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Students.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Validators;

public class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
{
    private static readonly string[] ValidStatuses = ["Active", "Inactive"];

    public UpdateStudentValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.EnrollmentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(2).MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).MaximumLength(255);
        RuleFor(x => x.PhoneNumber).MaximumLength(30).When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        RuleFor(x => x.Gender).MaximumLength(30).When(x => !string.IsNullOrWhiteSpace(x.Gender));
        RuleFor(x => x.Address).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Address));
        RuleFor(x => x.Status).Must(status => ValidStatuses.Contains(status, StringComparer.OrdinalIgnoreCase)).WithMessage("Status must be Active or Inactive.");
        RuleFor(x => x.AdmissionDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Admission date cannot be in the future.");
    }
}
