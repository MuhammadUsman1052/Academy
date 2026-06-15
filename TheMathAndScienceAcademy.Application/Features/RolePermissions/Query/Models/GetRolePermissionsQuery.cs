using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.RolePermissions.Query.Models;

public class GetRolePermissionsQuery : IRequest<ApiResponse<List<RolePermissionDto>>>
{
    public Guid RoleId { get; set; }
}
