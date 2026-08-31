using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Models;

public class ResendStudentCredentialsCommand : IRequest<ApiResponse<bool>>
{
    public Guid StudentId { get; set; }
}
