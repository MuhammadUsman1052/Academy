using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class StudentEnrollmentRepository : IStudentEnrollmentRepository
{
    private readonly AppDbContext _context;
    public StudentEnrollmentRepository(AppDbContext context) => _context = context;
    public async Task<StudentEnrollment?> CreateAsync(StudentEnrollment enrollment) { await _context.StudentEnrollments.AddAsync(enrollment); await _context.SaveChangesAsync(); return enrollment; }
    public Task<StudentEnrollment?> GetByIdAsync(string id) => _context.StudentEnrollments.FirstOrDefaultAsync(x => x.Id == id);
    public Task<List<StudentEnrollment>> GetByAcademyIdAsync(string academyId) => _context.StudentEnrollments.AsNoTracking().Where(x => x.AcademyId == academyId).OrderByDescending(x => x.EnrollmentDate).ToListAsync();
    public Task<List<StudentEnrollment>> GetByStudentIdAsync(string studentId, string academyId) => _context.StudentEnrollments.AsNoTracking().Where(x => x.StudentId == studentId && x.AcademyId == academyId).OrderByDescending(x => x.EnrollmentDate).ToListAsync();
    public Task<List<StudentEnrollment>> GetByClassIdAsync(string classId, string academyId) => _context.StudentEnrollments.AsNoTracking().Where(x => x.ClassId == classId && x.AcademyId == academyId).OrderBy(x => x.Status).ThenBy(x => x.StudentId).ToListAsync();
    public Task<StudentEnrollment?> GetActiveDuplicateAsync(string studentId, string academicYear, string academyId, string? excludeId = null) => _context.StudentEnrollments.AsNoTracking().FirstOrDefaultAsync(x => x.StudentId == studentId && x.AcademicYear.ToLower() == academicYear.ToLower() && x.AcademyId == academyId && x.Status == "Active" && (excludeId == null || x.Id != excludeId));
    public async Task<bool> UpdateAsync(StudentEnrollment enrollment) { _context.StudentEnrollments.Update(enrollment); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteAsync(string id) { var item = await _context.StudentEnrollments.FirstOrDefaultAsync(x => x.Id == id); if (item is null) return false; _context.StudentEnrollments.Remove(item); return await _context.SaveChangesAsync() > 0; }
}
