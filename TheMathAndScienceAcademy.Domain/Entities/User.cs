
using TheMathAndScienceAcademy.Domain.Common;
namespace TheMathAndScienceAcademy.Domain.Entities;
public class User : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiry { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public string RoleId { get; set; } = default!;
    public string? AcademyId { get; set; }
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
}
