namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface IRolePermissionSyncService
{
    Task SyncAllAsync();
    Task SyncRoleAsync(string roleId);
    Task SyncPermissionAsync(string permissionId);
}
