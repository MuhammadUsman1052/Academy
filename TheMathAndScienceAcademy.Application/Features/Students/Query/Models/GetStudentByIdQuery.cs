using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Students.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Students.Query.Models;

public class GetStudentByIdQuery : IRequest<ApiResponse<StudentDto>>
{
    public Guid Id { get; set; }
}
