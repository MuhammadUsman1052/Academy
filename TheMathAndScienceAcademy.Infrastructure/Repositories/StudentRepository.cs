using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context) => _context = context;

    public Task<List<Student>> GetByAcademyIdAsync(string academyId)
        => _context.Students.AsNoTracking().Where(x => x.AcademyId == academyId).OrderBy(x => x.FullName).ToListAsync();

    public Task<Student?> GetByIdAsync(string id)
        => _context.Students.FirstOrDefaultAsync(x => x.Id == id);

    public Task<Student?> GetByEnrollmentNumberAsync(string academyId, string enrollmentNumber)
        => _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.AcademyId == academyId && x.EnrollmentNumber.ToLower() == enrollmentNumber.ToLower());

    public Task<Student?> GetByUserIdAsync(string userId)
        => _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);

    public async Task<Student?> CreateAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (student is null) return false;

        var enrollments = await _context.StudentEnrollments
            .Where(x => x.StudentId == student.Id)
            .ToListAsync();
        _context.StudentEnrollments.RemoveRange(enrollments);
        _context.Students.Remove(student);
        return await _context.SaveChangesAsync() > 0;
    }
}
