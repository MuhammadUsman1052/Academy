
using TheMathAndScienceAcademy.Domain.Entities;
namespace TheMathAndScienceAcademy.Domain.Repositories;
public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<User?> GetByResetPasswordTokenAsync(string resetPasswordToken);
    Task CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
}
