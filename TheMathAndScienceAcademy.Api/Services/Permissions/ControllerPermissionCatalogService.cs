using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TheMathAndScienceAcademy.Api.Authorization;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Api.Services.Permissions;

public class ControllerPermissionCatalogService : IPermissionCatalogService
{
    public IReadOnlyCollection<string> GetPermissionNames()
    {
        return typeof(Program).Assembly
            .GetTypes()
            .Where(IsControllerWithPermissions)
            .SelectMany(GetPermissionNamesFromControllerType)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToArray();
    }

    private static bool IsControllerWithPermissions(Type type)
    {
        if (!type.IsClass || type.IsAbstract || !typeof(ControllerBase).IsAssignableFrom(type) || !type.Name.EndsWith("Controller", StringComparison.Ordinal))
        {
            return false;
        }

        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Any(method => method.GetCustomAttributes(typeof(HasPermissionAttribute), inherit: true).Any());
    }

    private static IEnumerable<string> GetPermissionNamesFromControllerType(Type controllerType)
    {
        return controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(method => method.GetCustomAttributes(typeof(HasPermissionAttribute), inherit: true)
                .OfType<HasPermissionAttribute>()
                .Select(attribute => attribute.PermissionName));
    }
}
