using Microsoft.AspNetCore.Authorization;

namespace TheMathAndScienceAcademy.Api.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission";
    public string PermissionName { get; }

    public HasPermissionAttribute(string permissionName)
    {
        PermissionName = permissionName;
        Policy = $"{PolicyPrefix}:{permissionName}";
    }
}
