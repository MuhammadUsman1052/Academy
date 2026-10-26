using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class TeachingAssignment : BaseEntity
{
    public string TeacherId { get; set; } = default!;
    public string SubjectId { get; set; } = default!;
    public string ClassId { get; set; } = default!;
    public string AcademyId { get; set; } = default!;
    public string AcademicYear { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public string AssignmentRole { get; set; } = "Primary";
}
