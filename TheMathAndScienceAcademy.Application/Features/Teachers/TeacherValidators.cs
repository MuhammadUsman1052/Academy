using FluentValidation;

namespace TheMathAndScienceAcademy.Application.Features.Teachers;
public class CreateTeacherValidator : AbstractValidator<CreateTeacherCommand>
{
    public CreateTeacherValidator(){RuleFor(x=>x.FullName).NotEmpty().MinimumLength(2).MaximumLength(200);RuleFor(x=>x.EmployeeNumber).NotEmpty().MaximumLength(50);RuleFor(x=>x.Email).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.Email));RuleFor(x=>x.PhoneNumber).MaximumLength(30);RuleFor(x=>x.Address).MaximumLength(500);RuleFor(x=>x.Status).Must(x=>x is "Active" or "Inactive").WithMessage("Status must be Active or Inactive.");RuleFor(x=>x.HireDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Date);}
}
public class UpdateTeacherValidator : AbstractValidator<UpdateTeacherCommand>{public UpdateTeacherValidator(){Include(new CreateTeacherValidator());RuleFor(x=>x.Id).NotEmpty();}}
