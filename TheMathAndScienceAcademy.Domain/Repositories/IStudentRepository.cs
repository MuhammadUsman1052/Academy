using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Domain.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetByAcademyIdAsync(string academyId);
    Task<Student?> GetByIdAsync(string id);
    Task<Student?> GetByEnrollmentNumberAsync(string academyId, string enrollmentNumber);
    Task<Student?> GetByUserIdAsync(string userId);
    Task<Student?> CreateAsync(Student student);
    Task<bool> UpdateAsync(Student student);
    Task<bool> DeleteAsync(string id);
}
