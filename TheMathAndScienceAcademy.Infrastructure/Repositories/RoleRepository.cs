using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

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

    public async Task<Role?> GetByNameAsync(string name, string? academyId = null)
        => await _context.Roles.FirstOrDefaultAsync(x => x.Name == name && x.AcademyId == academyId);

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

        var rolePermissions = await _context.RolePermissions
            .Where(x => x.RoleId == role.Id)
            .ToListAsync();
        _context.RolePermissions.RemoveRange(rolePermissions);

        var users = await _context.Users
            .Where(x => x.RoleId == role.Id)
            .ToListAsync();
        var userIds = users.Select(x => x.Id).ToList();
        if (userIds.Count > 0)
        {
            var linkedStudents = await _context.Students
                .Where(x => x.UserId != null && userIds.Contains(x.UserId))
                .ToListAsync();
            foreach (var student in linkedStudents)
                student.UserId = null;
        }
        _context.Users.RemoveRange(users);
        _context.Roles.Remove(role);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }
}
