using Microsoft.AspNetCore.Authorization;

namespace TheMathAndScienceAcademy.Api.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission";

    public HasPermissionAttribute(string permissionName)
    {
        Policy = $"{PolicyPrefix}:{permissionName}";
    }
}
