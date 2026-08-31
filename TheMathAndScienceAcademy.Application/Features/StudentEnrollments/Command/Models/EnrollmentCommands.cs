using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Command.Models;

public class CreateStudentEnrollmentCommand : IRequest<ApiResponse<StudentEnrollmentDto>>
{
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public string AcademicYear { get; set; } = default!;
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow.Date;
}

public class UpdateStudentEnrollmentCommand : CreateStudentEnrollmentCommand, IRequest<ApiResponse<StudentEnrollmentDto>>
{
    public Guid Id { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
}

public class DeleteStudentEnrollmentCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
