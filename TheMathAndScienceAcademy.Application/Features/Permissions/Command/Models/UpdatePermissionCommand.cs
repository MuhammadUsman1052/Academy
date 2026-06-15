using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Permissions.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;

public class UpdatePermissionCommand : IRequest<ApiResponse<PermissionDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
