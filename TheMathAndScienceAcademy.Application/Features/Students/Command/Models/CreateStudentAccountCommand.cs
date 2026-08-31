using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Students.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Models;

public class CreateStudentAccountCommand : IRequest<ApiResponse<StudentDto>>
{
    public Guid StudentId { get; set; }
}
