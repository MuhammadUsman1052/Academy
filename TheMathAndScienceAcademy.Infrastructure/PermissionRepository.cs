using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
        => await _context.Permissions.FirstOrDefaultAsync(x => x.Id == id.ToString());

    public async Task<List<Permission>> GetAllAsync()
        => await _context.Permissions.AsNoTracking().ToListAsync();

    public async Task<Permission?> GetByNameAsync(string name)
        => await _context.Permissions.FirstOrDefaultAsync(x => x.Name == name);

    public async Task<Permission?> CreateAsync(Permission permission)
    {
        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task<bool> UpdateAsync(Permission permission)
    {
        _context.Permissions.Update(permission);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == id.ToString());
        if (permission is null)
        {
            return false;
        }

        _context.Permissions.Remove(permission);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }
}
