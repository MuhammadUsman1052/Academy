namespace TheMathAndScienceAcademy.Application.Features.Classes.Dtos;

public class AcademicClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Grade { get; set; } = default!;
    public string Section { get; set; } = default!;
    public string AcademicYear { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = default!;
    public Guid AcademyId { get; set; }
    public string? AcademyName { get; set; }
}
