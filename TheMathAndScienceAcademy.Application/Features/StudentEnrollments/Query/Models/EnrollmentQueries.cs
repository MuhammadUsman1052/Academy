using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Query.Models;

public class GetStudentEnrollmentsQuery : IRequest<ApiResponse<List<StudentEnrollmentDto>>> { }

public class GetEnrollmentsByStudentQuery : IRequest<ApiResponse<List<StudentEnrollmentDto>>>
{
    public Guid StudentId { get; set; }
}

public class GetEnrollmentsByClassQuery : IRequest<ApiResponse<List<StudentEnrollmentDto>>>
{
    public Guid ClassId { get; set; }
}
