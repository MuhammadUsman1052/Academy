using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
        var academy = await _context.Academies.FirstOrDefaultAsync(x => x.Id == id.ToString());
        if (academy is null)
        {
            return false;
        }

        _context.Academies.Remove(academy);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }
}
