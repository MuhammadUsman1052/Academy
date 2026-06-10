using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
        => await _context.Roles.FirstOrDefaultAsync(x => x.Id == id.ToString());

    public async Task<List<Role>> GetAllAsync()
        => await _context.Roles.AsNoTracking().ToListAsync();

    public async Task<Role?> GetByNameAsync(string name)
        => await _context.Roles.FirstOrDefaultAsync(x => x.Name == name);

    public async Task<Role?> CreateAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<bool> UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id.ToString());
        if (role is null)
        {
            return false;
        }

        _context.Roles.Remove(role);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }
}
