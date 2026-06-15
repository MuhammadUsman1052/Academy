
using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Repositories;
using TheMathAndScienceAcademy.Domain.Entities;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;
    public async Task<List<User>> GetAllAsync() => await _context.Users.AsNoTracking().ToListAsync();
    public async Task<User?> GetByIdAsync(string id) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    public async Task<User?> GetByEmailAsync(string email) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
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
}
