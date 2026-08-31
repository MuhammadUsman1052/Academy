using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class StudentEnrollment : BaseEntity
{
    public string StudentId { get; set; } = default!;
    public string ClassId { get; set; } = default!;
    public string AcademyId { get; set; } = default!;
    public string AcademicYear { get; set; } = default!;
    public DateTime EnrollmentDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
}
