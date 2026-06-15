using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id);
    Task<List<Permission>> GetAllAsync();
    Task<Permission?> GetByNameAsync(string name);
    Task<Permission?> CreateAsync(Permission permission);
    Task<bool> UpdateAsync(Permission permission);
    Task<bool> DeleteAsync(Guid id);
}
