using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Models;

public class DeleteStudentCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
