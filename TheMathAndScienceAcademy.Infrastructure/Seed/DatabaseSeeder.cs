using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using BCryptNet = BCrypt.Net.BCrypt;

public class DatabaseSeeder
{
    private const string SuperAdminRoleName = "SuperAdmin";
    private const string SuperAdminEmail = "admin@academy.com";

    private readonly AppDbContext _context;

    public DatabaseSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedSuperAdminRoleAsync()
    {
        var roleExists = await _context.Roles
            .AnyAsync(x => x.Name.ToLower() == SuperAdminRoleName.ToLower());

        if (roleExists)
        {
            return;
        }

        await _context.Roles.AddAsync(new Role
        {
            Name = SuperAdminRoleName,
            Description = "System default super administrator role"
        });

        await _context.SaveChangesAsync();
    }

    public async Task SeedSuperAdminUserAsync()
    {
        var superAdminRole = await _context.Roles
            .FirstOrDefaultAsync(x => x.Name.ToLower() == SuperAdminRoleName.ToLower());

        if (superAdminRole is null)
        {
            return;
        }

        var userExists = await _context.Users
            .AnyAsync(x => x.Email.ToLower() == SuperAdminEmail.ToLower());

        if (userExists)
        {
            return;
        }

        await _context.Users.AddAsync(new User
        {
            Name = "System Administrator",
            Email = SuperAdminEmail,
            PasswordHash = BCryptNet.HashPassword("Admin@123"),
            RoleId = superAdminRole.Id,
            IsActive = true,
            MustChangePassword = false
        });

        await _context.SaveChangesAsync();
    }
}
