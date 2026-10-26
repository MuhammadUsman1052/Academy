using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public string AcademicYear { get; set; } = default!;
    public string Status { get; set; } = "Active";
    public string AcademyId { get; set; } = default!;
}
