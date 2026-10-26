using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class TeachingAssignmentRepository : ITeachingAssignmentRepository
{
    private readonly AppDbContext _context;
    public TeachingAssignmentRepository(AppDbContext context) => _context = context;
    public Task<List<TeachingAssignment>> GetByAcademyIdAsync(string academyId) => _context.TeachingAssignments.AsNoTracking().Where(x => x.AcademyId == academyId).OrderByDescending(x => x.CreatedAt).ToListAsync();
    public Task<List<TeachingAssignment>> GetByTeacherIdAsync(string academyId, string teacherId) => _context.TeachingAssignments.AsNoTracking().Where(x => x.AcademyId == academyId && x.TeacherId == teacherId).ToListAsync();
    public Task<List<TeachingAssignment>> GetByClassIdAsync(string academyId, string classId) => _context.TeachingAssignments.AsNoTracking().Where(x => x.AcademyId == academyId && x.ClassId == classId).ToListAsync();
    public Task<TeachingAssignment?> GetByIdAsync(string id) => _context.TeachingAssignments.FirstOrDefaultAsync(x => x.Id == id);
    public Task<TeachingAssignment?> GetActivePrimaryAsync(string academyId, string subjectId, string classId, string academicYear, string? excludeId = null) => _context.TeachingAssignments.AsNoTracking().FirstOrDefaultAsync(x => x.AcademyId == academyId && x.SubjectId == subjectId && x.ClassId == classId && x.AcademicYear.ToLower() == academicYear.ToLower() && x.Status == "Active" && x.AssignmentRole == "Primary" && (excludeId == null || x.Id != excludeId));
    public Task<TeachingAssignment?> GetOverlappingSubstituteAsync(string academyId, string subjectId, string classId, string academicYear, DateTime startDate, DateTime endDate, string? excludeId = null) => _context.TeachingAssignments.AsNoTracking().FirstOrDefaultAsync(x => x.AcademyId == academyId && x.SubjectId == subjectId && x.ClassId == classId && x.AcademicYear.ToLower() == academicYear.ToLower() && x.Status == "Active" && x.AssignmentRole == "Substitute" && x.EndDate != null && x.StartDate <= endDate && x.EndDate >= startDate && (excludeId == null || x.Id != excludeId));
    public Task<TeachingAssignment?> GetDuplicateAsync(string academyId, string teacherId, string subjectId, string classId, string academicYear, string assignmentRole, string? excludeId = null) => _context.TeachingAssignments.AsNoTracking().FirstOrDefaultAsync(x => x.AcademyId == academyId && x.TeacherId == teacherId && x.SubjectId == subjectId && x.ClassId == classId && x.AcademicYear.ToLower() == academicYear.ToLower() && x.Status == "Active" && x.AssignmentRole == assignmentRole && (excludeId == null || x.Id != excludeId));
    public async Task<TeachingAssignment?> CreateAsync(TeachingAssignment assignment) { await _context.TeachingAssignments.AddAsync(assignment); await _context.SaveChangesAsync(); return assignment; }
    public async Task<bool> UpdateAsync(TeachingAssignment assignment) { _context.TeachingAssignments.Update(assignment); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteAsync(string id) { var item = await _context.TeachingAssignments.FirstOrDefaultAsync(x => x.Id == id); if (item is null) return false; _context.TeachingAssignments.Remove(item); return await _context.SaveChangesAsync() > 0; }
}
