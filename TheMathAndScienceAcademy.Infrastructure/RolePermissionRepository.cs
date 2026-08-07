using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AppDbContext _context;

    public RolePermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AssignPermissionAsync(string roleId, string permissionId)
    {
        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);

        if (rolePermission is null)
        {
            rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                IsGranted = true
            };

            await _context.RolePermissions.AddAsync(rolePermission);
        }
        else
        {
            rolePermission.IsGranted = true;
        }

        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> RemovePermissionAsync(string roleId, string permissionId)
    {
        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);

        if (rolePermission is null)
        {
            rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                IsGranted = false
            };

            await _context.RolePermissions.AddAsync(rolePermission);
        }
        else
        {
            rolePermission.IsGranted = false;
        }

        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<List<RolePermission>> GetPermissionsByRoleIdAsync(string roleId)
        => await _context.RolePermissions
            .AsNoTracking()
            .Include(x => x.Permission)
            .Where(x => x.RoleId == roleId)
            .OrderBy(x => x.Permission.Name)
            .ToListAsync();

    public async Task<bool> RoleHasPermissionAsync(string roleId, string permissionName)
        => await _context.RolePermissions
            .AsNoTracking()
            .AnyAsync(x => x.RoleId == roleId && x.IsGranted && x.Permission.Name == permissionName);
}
