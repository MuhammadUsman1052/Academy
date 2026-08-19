using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<List<User>> GetAllAsync() => await _context.Users.AsNoTracking().ToListAsync();

    public async Task<List<User>> GetByAcademyIdAsync(string academyId)
        => await _context.Users.AsNoTracking().Where(x => x.AcademyId == academyId).ToListAsync();

    public async Task<User?> GetByIdAsync(string id) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

    public async Task<User?> GetByResetPasswordTokenAsync(string resetPasswordToken) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.ResetPasswordToken == resetPasswordToken);

    public async Task CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }
}
