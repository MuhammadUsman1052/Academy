namespace TheMathAndScienceAcademy.Application.Features.Auth.Dtos;

public class CurrentUserDto
{
    public string UserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public string? AcademyId { get; set; }
    public bool MustChangePassword { get; set; }
}
