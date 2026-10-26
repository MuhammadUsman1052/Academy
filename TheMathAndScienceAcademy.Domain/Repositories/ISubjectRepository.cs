using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface ISubjectRepository
{
    Task<List<Subject>> GetByAcademyIdAsync(string academyId);
    Task<Subject?> GetByIdAsync(string id);
    Task<Subject?> GetDuplicateAsync(string academyId, string name, string code, string academicYear, string? excludeId = null);
    Task<Subject?> CreateAsync(Subject subject);
    Task<bool> UpdateAsync(Subject subject);
    Task<bool> DeleteAsync(string id);
}
