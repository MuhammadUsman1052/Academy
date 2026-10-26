using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Repositories;

public class AcademyRepository : IAcademyRepository
{
    private readonly AppDbContext _context;

    public AcademyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Academy?> CreateAsync(Academy academy)
    {
        await _context.Academies.AddAsync(academy);
        await _context.SaveChangesAsync();
        return academy;
    }

    public async Task<Academy?> GetByIdAsync(Guid id)
        => await _context.Academies.FirstOrDefaultAsync(x => x.Id == id.ToString());

    public async Task<List<Academy>> GetAllAsync()
        => await _context.Academies.AsNoTracking().ToListAsync();

    public async Task<bool> UpdateAsync(Academy academy)
    {
        _context.Academies.Update(academy);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var academyId = id.ToString();
        var academy = await _context.Academies.FirstOrDefaultAsync(x => x.Id == academyId);
        if (academy is null)
        {
            return false;
        }

        var studentIds = await _context.Students
            .Where(x => x.AcademyId == academyId)
            .Select(x => x.Id)
            .ToListAsync();

        var classIds = await _context.AcademicClasses
            .Where(x => x.AcademyId == academyId)
            .Select(x => x.Id)
            .ToListAsync();

        var roleIds = await _context.Roles
            .Where(x => x.AcademyId == academyId)
            .Select(x => x.Id)
            .ToListAsync();

        if (studentIds.Count > 0)
        {
            var enrollments = await _context.StudentEnrollments
                .Where(x => studentIds.Contains(x.StudentId))
                .ToListAsync();
            _context.StudentEnrollments.RemoveRange(enrollments);
        }

        if (classIds.Count > 0)
        {
            var enrollments = await _context.StudentEnrollments
                .Where(x => classIds.Contains(x.ClassId))
                .ToListAsync();
            _context.StudentEnrollments.RemoveRange(enrollments);
        }

        var users = await _context.Users
            .Where(x => x.AcademyId == academyId)
            .ToListAsync();
        var userIds = users.Select(x => x.Id).ToList();
        if (userIds.Count > 0)
        {
            var linkedStudents = await _context.Students
                .Where(x => x.UserId != null && userIds.Contains(x.UserId))
                .ToListAsync();
            _context.Students.RemoveRange(linkedStudents);
        }
        _context.Users.RemoveRange(users);

        if (roleIds.Count > 0)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(x => roleIds.Contains(x.RoleId))
                .ToListAsync();
            _context.RolePermissions.RemoveRange(rolePermissions);
        }

        var students = await _context.Students
            .Where(x => x.AcademyId == academyId)
            .ToListAsync();
        _context.Students.RemoveRange(students);

        var classes = await _context.AcademicClasses
            .Where(x => x.AcademyId == academyId)
            .ToListAsync();
        _context.AcademicClasses.RemoveRange(classes);

        var roles = await _context.Roles
            .Where(x => x.AcademyId == academyId)
            .ToListAsync();
        _context.Roles.RemoveRange(roles);
        _context.Academies.Remove(academy);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }
}
