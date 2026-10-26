namespace TheMathAndScienceAcademy.Application.Features.Auth.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
    public string? AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public List<string> Permissions { get; set; } = new();
    public bool MustChangePassword { get; set; }
}
