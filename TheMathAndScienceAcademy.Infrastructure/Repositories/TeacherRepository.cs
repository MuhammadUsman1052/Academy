using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _context;
    public TeacherRepository(AppDbContext context) => _context = context;
    public Task<List<Teacher>> GetByAcademyIdAsync(string academyId) => _context.Teachers.AsNoTracking().Where(x => x.AcademyId == academyId).OrderBy(x => x.FullName).ToListAsync();
    public Task<Teacher?> GetByIdAsync(string id) => _context.Teachers.FirstOrDefaultAsync(x => x.Id == id);
    public Task<Teacher?> GetByUserIdAsync(string userId) => _context.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
    public Task<Teacher?> GetByEmployeeNumberAsync(string academyId, string employeeNumber, string? excludeId = null) => _context.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.AcademyId == academyId && x.EmployeeNumber.ToLower() == employeeNumber.ToLower() && (excludeId == null || x.Id != excludeId));
    public async Task<Teacher?> CreateAsync(Teacher teacher) { await _context.Teachers.AddAsync(teacher); await _context.SaveChangesAsync(); return teacher; }
    public async Task<bool> UpdateAsync(Teacher teacher) { _context.Teachers.Update(teacher); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> DeleteAsync(string id) { var item = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == id); if (item is null) return false; _context.Teachers.Remove(item); return await _context.SaveChangesAsync() > 0; }
}
