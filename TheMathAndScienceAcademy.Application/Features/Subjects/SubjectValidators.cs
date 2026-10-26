using FluentValidation;
namespace TheMathAndScienceAcademy.Application.Features.Subjects;
public class CreateSubjectValidator:AbstractValidator<CreateSubjectCommand>{public CreateSubjectValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(150);RuleFor(x=>x.Code).NotEmpty().MaximumLength(50);RuleFor(x=>x.AcademicYear).NotEmpty().MaximumLength(30);RuleFor(x=>x.Description).MaximumLength(500);RuleFor(x=>x.Status).Must(x=>x is "Active" or "Inactive").WithMessage("Status must be Active or Inactive.");}}
public class UpdateSubjectValidator:AbstractValidator<UpdateSubjectCommand>{public UpdateSubjectValidator(){Include(new CreateSubjectValidator());RuleFor(x=>x.Id).NotEmpty();}}
