using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Infrastructure.Services;

public class RolePermissionSyncService : IRolePermissionSyncService
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private readonly AppDbContext _context;
    private readonly IPermissionCatalogService _permissionCatalogService;

    public RolePermissionSyncService(AppDbContext context, IPermissionCatalogService permissionCatalogService)
    {
        _context = context;
        _permissionCatalogService = permissionCatalogService;
    }

    public async Task SyncAllAsync()
    {
        await SyncMissingPermissionsAsync();
        await SyncExistingRolePermissionRowsAsync();
    }

    public async Task SyncRoleAsync(string roleId)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
        if (role is null)
        {
            return;
        }

        var permissions = await _context.Permissions.AsNoTracking().ToListAsync();
        if (permissions.Count == 0)
        {
            return;
        }

        var existingPermissionIds = await _context.RolePermissions
            .Where(x => x.RoleId == roleId)
            .Select(x => x.PermissionId)
            .ToListAsync();

        var existingSet = existingPermissionIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var isSuperAdmin = string.Equals(role.Name, SuperAdminRoleName, StringComparison.OrdinalIgnoreCase);
        var changed = false;

        foreach (var permission in permissions)
        {
            if (existingSet.Contains(permission.Id))
            {
                continue;
            }

            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id,
                IsGranted = isSuperAdmin
            });
            changed = true;
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        if (isSuperAdmin)
        {
            var superAdminLinks = await _context.RolePermissions
                .Where(x => x.RoleId == role.Id && !x.IsGranted)
                .ToListAsync();

            if (superAdminLinks.Count > 0)
            {
                foreach (var link in superAdminLinks)
                {
                    link.IsGranted = true;
                }

                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task SyncPermissionAsync(string permissionId)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == permissionId);
        if (permission is null)
        {
            return;
        }

        var roles = await _context.Roles.AsNoTracking().ToListAsync();
        if (roles.Count == 0)
        {
            return;
        }

        var existingRoleIds = await _context.RolePermissions
            .Where(x => x.PermissionId == permissionId)
            .Select(x => x.RoleId)
            .ToListAsync();

        var existingSet = existingRoleIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var changed = false;

        foreach (var role in roles)
        {
            if (existingSet.Contains(role.Id))
            {
                continue;
            }

            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id,
                IsGranted = string.Equals(role.Name, SuperAdminRoleName, StringComparison.OrdinalIgnoreCase)
            });
            changed = true;
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        var superAdminRole = roles.FirstOrDefault(x => string.Equals(x.Name, SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));
        if (superAdminRole is null)
        {
            return;
        }

        var superAdminLink = await _context.RolePermissions
            .FirstOrDefaultAsync(x => x.RoleId == superAdminRole.Id && x.PermissionId == permissionId);

        if (superAdminLink is not null && !superAdminLink.IsGranted)
        {
            superAdminLink.IsGranted = true;
            await _context.SaveChangesAsync();
        }
    }

    private async Task SyncMissingPermissionsAsync()
    {
        var permissionNames = _permissionCatalogService.GetPermissionNames();
        if (permissionNames.Count == 0)
        {
            return;
        }

        var existingPermissionNames = await _context.Permissions
            .AsNoTracking()
            .Select(x => x.Name)
            .ToListAsync();

        var existingSet = existingPermissionNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var changed = false;

        foreach (var permissionName in permissionNames)
        {
            if (existingSet.Contains(permissionName))
            {
                continue;
            }

            await _context.Permissions.AddAsync(new Permission
            {
                Name = permissionName,
                Description = $"{PermissionNameParser.GetModuleName(permissionName)} endpoint"
            });
            changed = true;
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }
    }

    private async Task SyncExistingRolePermissionRowsAsync()
    {
        var roles = await _context.Roles.AsNoTracking().ToListAsync();
        var permissions = await _context.Permissions.AsNoTracking().ToListAsync();

        if (roles.Count == 0 || permissions.Count == 0)
        {
            return;
        }

        var existingLinks = await _context.RolePermissions.ToListAsync();

        var existingLookup = existingLinks
            .ToDictionary(x => $"{x.RoleId}:{x.PermissionId}", StringComparer.OrdinalIgnoreCase);

        var changed = false;
        foreach (var role in roles)
        {
            var isSuperAdmin = string.Equals(role.Name, SuperAdminRoleName, StringComparison.OrdinalIgnoreCase);

            foreach (var permission in permissions)
            {
                var key = $"{role.Id}:{permission.Id}";
                if (existingLookup.ContainsKey(key))
                {
                    if (isSuperAdmin)
                    {
                        var existingLink = existingLinks.FirstOrDefault(x => x.RoleId == role.Id && x.PermissionId == permission.Id);
                        if (existingLink is not null && !existingLink.IsGranted)
                        {
                            existingLink.IsGranted = true;
                            changed = true;
                        }
                    }

                    continue;
                }

                await _context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id,
                    IsGranted = isSuperAdmin
                });
                changed = true;
            }
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }
    }

}
