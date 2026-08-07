using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface IRolePermissionRepository
{
    Task<bool> AssignPermissionAsync(string roleId, string permissionId);
    Task<bool> RemovePermissionAsync(string roleId, string permissionId);
    Task<List<RolePermission>> GetPermissionsByRoleIdAsync(string roleId);
    Task<bool> RoleHasPermissionAsync(string roleId, string permissionName);
}
