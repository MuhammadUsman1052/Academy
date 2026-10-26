using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;
    public SubjectRepository(AppDbContext context) => _context = context;
    public Task<List<Subject>> GetByAcademyIdAsync(string academyId) => _context.Subjects.AsNoTracking().Where(x => x.AcademyId == academyId).OrderBy(x => x.Name).ToListAsync();
    public Task<Subject?> GetByIdAsync(string id) => _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);
    public Task<Subject?> GetDuplicateAsync(string academyId, string name, string code, string academicYear, string? excludeId = null) => _context.Subjects.AsNoTracking().FirstOrDefaultAsync(x => x.AcademyId == academyId && x.AcademicYear.ToLower() == academicYear.ToLower() && (x.Name.ToLower() == name.ToLower() || x.Code.ToLower() == code.ToLower()) && (excludeId == null || x.Id != excludeId));
    public async Task<Subject?> CreateAsync(Subject subject) { await _context.Subjects.AddAsync(subject); await _context.SaveChangesAsync(); return subject; }
    public async Task<bool> UpdateAsync(Subject subject) { _context.Subjects.Update(subject); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteAsync(string id) { var item = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id); if (item is null) return false; _context.Subjects.Remove(item); return await _context.SaveChangesAsync() > 0; }
}
