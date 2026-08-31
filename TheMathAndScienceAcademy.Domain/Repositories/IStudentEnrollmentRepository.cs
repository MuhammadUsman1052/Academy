using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface IStudentEnrollmentRepository
{
    Task<StudentEnrollment?> CreateAsync(StudentEnrollment enrollment);
    Task<StudentEnrollment?> GetByIdAsync(string id);
    Task<List<StudentEnrollment>> GetByAcademyIdAsync(string academyId);
    Task<List<StudentEnrollment>> GetByStudentIdAsync(string studentId, string academyId);
    Task<List<StudentEnrollment>> GetByClassIdAsync(string classId, string academyId);
    Task<StudentEnrollment?> GetActiveDuplicateAsync(string studentId, string academicYear, string academyId, string? excludeId = null);
    Task<bool> UpdateAsync(StudentEnrollment enrollment);
    Task<bool> DeleteAsync(string id);
}
