namespace TheMathAndScienceAcademy.Application.Common;

public static class PermissionNameParser
{
    public static string GetModuleName(string permissionName)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
        {
            return string.Empty;
        }

        var separatorIndex = permissionName.IndexOf('.');
        if (separatorIndex <= 0)
        {
            return permissionName;
        }

        return permissionName[..separatorIndex];
    }
}
