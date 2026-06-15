namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string roleId, string permissionName);
}
