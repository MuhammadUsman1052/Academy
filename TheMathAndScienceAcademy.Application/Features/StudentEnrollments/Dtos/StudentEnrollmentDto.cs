namespace TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Dtos;

public class StudentEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public Guid ClassId { get; set; }
    public string? ClassName { get; set; }
    public string AcademicYear { get; set; } = default!;
    public DateTime EnrollmentDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = default!;
    public Guid AcademyId { get; set; }
}
