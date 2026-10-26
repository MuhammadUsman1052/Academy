using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class AcademicClassRepository : IAcademicClassRepository
{
    private readonly AppDbContext _context;
    public AcademicClassRepository(AppDbContext context) => _context = context;

    public async Task<AcademicClass?> CreateAsync(AcademicClass academicClass)
    {
        await _context.AcademicClasses.AddAsync(academicClass);
        await _context.SaveChangesAsync();
        return academicClass;
    }

    public Task<AcademicClass?> GetByIdAsync(string id) => _context.AcademicClasses.FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<AcademicClass>> GetByAcademyIdAsync(string academyId)
        => _context.AcademicClasses.AsNoTracking().Where(x => x.AcademyId == academyId).OrderBy(x => x.Grade).ThenBy(x => x.Section).ToListAsync();

    public Task<AcademicClass?> GetDuplicateAsync(string academyId, string grade, string section, string academicYear, string? excludeId = null)
        => _context.AcademicClasses.AsNoTracking().FirstOrDefaultAsync(x => x.AcademyId == academyId && x.Grade.ToLower() == grade.ToLower() && x.Section.ToLower() == section.ToLower() && x.AcademicYear.ToLower() == academicYear.ToLower() && (excludeId == null || x.Id != excludeId));

    public async Task<bool> UpdateAsync(AcademicClass academicClass)
    {
        _context.AcademicClasses.Update(academicClass);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var item = await _context.AcademicClasses.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return false;

        var enrollments = await _context.StudentEnrollments
            .Where(x => x.ClassId == item.Id)
            .ToListAsync();
        _context.StudentEnrollments.RemoveRange(enrollments);
        _context.AcademicClasses.Remove(item);
        return await _context.SaveChangesAsync() > 0;
    }
}
