using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Students.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Models;

public class CreateStudentCommand : IRequest<ApiResponse<StudentDto>>
{
    public string EnrollmentNumber { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "Active";
    public Guid? AcademyId { get; set; }
}
