using FluentValidation;
namespace TheMathAndScienceAcademy.Application.Features.TeachingAssignments;
public class CreateTeachingAssignmentValidator:AbstractValidator<CreateTeachingAssignmentCommand>{public CreateTeachingAssignmentValidator(){RuleFor(x=>x.TeacherId).NotEmpty();RuleFor(x=>x.SubjectId).NotEmpty();RuleFor(x=>x.ClassId).NotEmpty();RuleFor(x=>x.AcademicYear).NotEmpty().MaximumLength(30);RuleFor(x=>x.StartDate).NotEmpty();RuleFor(x=>x.Status).Must(x=>x is "Active" or "Inactive").WithMessage("Status must be Active or Inactive.");}}
public class UpdateTeachingAssignmentValidator:AbstractValidator<UpdateTeachingAssignmentCommand>{public UpdateTeachingAssignmentValidator(){Include(new CreateTeachingAssignmentValidator());RuleFor(x=>x.Id).NotEmpty();}}
