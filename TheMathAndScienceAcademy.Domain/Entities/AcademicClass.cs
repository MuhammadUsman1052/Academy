using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class AcademicClass : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Grade { get; set; } = default!;
    public string Section { get; set; } = default!;
    public string AcademicYear { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = "Active";
    public string AcademyId { get; set; } = default!;
}
