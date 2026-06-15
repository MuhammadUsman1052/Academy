namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? RoleId { get; }
    string? AcademyId { get; }
}
