using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;

public class DeleteAcademyCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}