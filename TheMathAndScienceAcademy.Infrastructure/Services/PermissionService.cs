using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public PermissionService(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<bool> HasPermissionAsync(string roleId, string permissionName)
        => await _rolePermissionRepository.RoleHasPermissionAsync(roleId, permissionName);
}
