using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface IAcademicClassRepository
{
    Task<AcademicClass?> CreateAsync(AcademicClass academicClass);
    Task<AcademicClass?> GetByIdAsync(string id);
    Task<List<AcademicClass>> GetByAcademyIdAsync(string academyId);
    Task<AcademicClass?> GetDuplicateAsync(string academyId, string grade, string section, string academicYear, string? excludeId = null);
    Task<bool> UpdateAsync(AcademicClass academicClass);
    Task<bool> DeleteAsync(string id);
}
