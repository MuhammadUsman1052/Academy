
using TheMathAndScienceAcademy.Domain.Entities;
namespace TheMathAndScienceAcademy.Domain.Repositories;
public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user);
}
