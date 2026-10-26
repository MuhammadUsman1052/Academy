using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class Teacher : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string EmployeeNumber { get; set; } = default!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime HireDate { get; set; }
    public string Status { get; set; } = "Active";
    public string AcademyId { get; set; } = default!;
    public string? UserId { get; set; }
    public string? ProfilePhotoKey { get; set; }
    public string? ProfilePhotoContentType { get; set; }
    public DateTime? ProfilePhotoUpdatedAt { get; set; }
}
