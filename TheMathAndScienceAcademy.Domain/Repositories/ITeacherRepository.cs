using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface ITeacherRepository
{
    Task<List<Teacher>> GetByAcademyIdAsync(string academyId);
    Task<Teacher?> GetByIdAsync(string id);
    Task<Teacher?> GetByUserIdAsync(string userId);
    Task<Teacher?> GetByEmployeeNumberAsync(string academyId, string employeeNumber, string? excludeId = null);
    Task<Teacher?> CreateAsync(Teacher teacher);
    Task<bool> UpdateAsync(Teacher teacher);
    Task<bool> DeleteAsync(string id);
}
