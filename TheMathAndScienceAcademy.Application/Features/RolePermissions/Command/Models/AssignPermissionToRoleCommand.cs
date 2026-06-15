using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Application.Features.RolePermissions.Command.Models;

public class AssignPermissionToRoleCommand : IRequest<ApiResponse<bool>>
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
