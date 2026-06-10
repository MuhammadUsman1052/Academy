using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Roles.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

public class CreateRoleCommand : IRequest<ApiResponse<RoleDto>>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
