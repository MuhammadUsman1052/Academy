using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Classes.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Classes.Command.Validators;

public class CreateAcademicClassValidator : AbstractValidator<CreateAcademicClassCommand>
{
    private static readonly string[] Statuses = ["Active", "Inactive"];
    public CreateAcademicClassValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Grade).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AcademicYear).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Status).Must(x => Statuses.Contains(x, StringComparer.OrdinalIgnoreCase)).WithMessage("Status must be Active or Inactive.");
        RuleFor(x => x.AcademyId).Must(x => x is null || x != Guid.Empty).WithMessage("Academy id must be a valid GUID.");
    }
}

public class UpdateAcademicClassValidator : AbstractValidator<UpdateAcademicClassCommand>
{
    public UpdateAcademicClassValidator()
    {
        Include(new CreateAcademicClassValidator());
        RuleFor(x => x.Id).NotEmpty();
    }
}
