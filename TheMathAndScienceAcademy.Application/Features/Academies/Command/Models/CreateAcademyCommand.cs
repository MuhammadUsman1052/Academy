using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;

public class CreateAcademyCommand : IRequest<ApiResponse<AcademyDto>>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string AdminName { get; set; } = default!;
    public string AdminEmail { get; set; } = default!;
}
