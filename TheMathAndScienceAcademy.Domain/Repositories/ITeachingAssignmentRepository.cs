using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface ITeachingAssignmentRepository
{
    Task<List<TeachingAssignment>> GetByAcademyIdAsync(string academyId);
    Task<List<TeachingAssignment>> GetByTeacherIdAsync(string academyId, string teacherId);
    Task<List<TeachingAssignment>> GetByClassIdAsync(string academyId, string classId);
    Task<TeachingAssignment?> GetByIdAsync(string id);
    Task<TeachingAssignment?> GetActivePrimaryAsync(string academyId, string subjectId, string classId, string academicYear, string? excludeId = null);
    Task<TeachingAssignment?> GetOverlappingSubstituteAsync(string academyId, string subjectId, string classId, string academicYear, DateTime startDate, DateTime endDate, string? excludeId = null);
    Task<TeachingAssignment?> GetDuplicateAsync(string academyId, string teacherId, string subjectId, string classId, string academicYear, string assignmentRole, string? excludeId = null);
    Task<TeachingAssignment?> CreateAsync(TeachingAssignment assignment);
    Task<bool> UpdateAsync(TeachingAssignment assignment);
    Task<bool> DeleteAsync(string id);
}
