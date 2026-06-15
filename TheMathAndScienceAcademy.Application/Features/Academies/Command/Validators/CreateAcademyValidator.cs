using FluentValidation;
using TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Command.Validators;

public class CreateAcademyValidator : AbstractValidator<CreateAcademyCommand>
{
    public CreateAcademyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Academy name is required")
            .MinimumLength(2).WithMessage("Academy name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Academy name must not exceed 200 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Academy email is required")
            .MaximumLength(256).WithMessage("Academy email must not exceed 256 characters")
            .EmailAddress().WithMessage("Academy email format is invalid");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MinimumLength(7).WithMessage("Phone number must be at least 7 characters")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MinimumLength(5).WithMessage("Address must be at least 5 characters")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.AdminName)
            .NotEmpty().WithMessage("Admin name is required")
            .MinimumLength(2).WithMessage("Admin name must be at least 2 characters")
            .MaximumLength(150).WithMessage("Admin name must not exceed 150 characters");

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Admin email is required")
            .MaximumLength(256).WithMessage("Admin email must not exceed 256 characters")
            .EmailAddress().WithMessage("Admin email format is invalid");
    }
}