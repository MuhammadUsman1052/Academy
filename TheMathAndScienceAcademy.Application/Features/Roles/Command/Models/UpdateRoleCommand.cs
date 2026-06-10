using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Roles.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;

public class UpdateRoleCommand : IRequest<ApiResponse<RoleDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
