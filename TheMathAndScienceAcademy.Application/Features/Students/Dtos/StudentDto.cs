namespace TheMathAndScienceAcademy.Application.Features.Students.Dtos;

public class StudentDto
{
    public Guid Id { get; set; }
    public string EnrollmentNumber { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime AdmissionDate { get; set; }
    public string Status { get; set; } = default!;
    public Guid AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool LoginEnabled { get; set; }
    public bool MustChangePassword { get; set; }
    public DateTime? CredentialsSentAt { get; set; }
    public bool CredentialsPending { get; set; }
    public bool HasProfilePhoto { get; set; }
    public DateTime? ProfilePhotoUpdatedAt { get; set; }
}
