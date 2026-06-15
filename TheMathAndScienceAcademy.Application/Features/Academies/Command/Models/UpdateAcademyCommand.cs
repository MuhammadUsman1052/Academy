using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;

public class UpdateAcademyCommand : IRequest<ApiResponse<AcademyDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Address { get; set; } = default!;
}