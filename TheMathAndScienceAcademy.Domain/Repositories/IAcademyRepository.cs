using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface IAcademyRepository
{
    Task<Academy?> GetByIdAsync(Guid id);
    Task<List<Academy>> GetAllAsync();
    Task<Academy?> CreateAsync(Academy academy);
    Task<bool> UpdateAsync(Academy academy);
    Task<bool> DeleteAsync(Guid id);
}
