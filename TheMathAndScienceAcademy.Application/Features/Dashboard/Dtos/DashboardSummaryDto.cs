namespace TheMathAndScienceAcademy.Application.Features.Dashboard.Dtos;

public class DashboardSummaryDto
{
    public string? AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public int TotalAcademies { get; set; }
    public int UserCount { get; set; }
    public int StudentCount { get; set; }
    public int ClassCount { get; set; }
    public int EnrollmentCount { get; set; }
}
