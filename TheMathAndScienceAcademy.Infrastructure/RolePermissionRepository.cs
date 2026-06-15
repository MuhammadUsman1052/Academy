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
        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        await _context.RolePermissions.AddAsync(rolePermission);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> RemovePermissionAsync(string roleId, string permissionId)
    {
        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);

        if (rolePermission is null)
        {
            return false;
        }

        _context.RolePermissions.Remove(rolePermission);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<List<Permission>> GetPermissionsByRoleIdAsync(string roleId)
        => await _context.RolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => x.Permission)
            .ToListAsync();

    public async Task<bool> RoleHasPermissionAsync(string roleId, string permissionName)
        => await _context.RolePermissions
            .AsNoTracking()
            .AnyAsync(x => x.RoleId == roleId && x.Permission.Name == permissionName);
}
